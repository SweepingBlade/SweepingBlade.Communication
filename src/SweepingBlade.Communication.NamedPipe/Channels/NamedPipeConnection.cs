using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Pipes;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using SweepingBlade.Communication.Primitives.Channels;
using SweepingBlade.Communication.Primitives.Dispatcher;

namespace SweepingBlade.Communication.NamedPipe.Channels;

public abstract class NamedPipeConnection : CommunicationContext, IConnection
{
    protected static readonly byte[] EndOfStream = { 0x1A };

    public abstract Task HandleAsync(ReadOnlyMemory<byte> data, CancellationToken cancellationToken);
    public abstract object Invoke(object obj, MethodInfo targetMethod, object[] args);
    public abstract Task InvokeAsync(object obj, MethodInfo targetMethod, object[] args);
    public abstract Task<T> InvokeAsync<T>(object obj, MethodInfo targetMethod, object[] args);
    public abstract Task SendAsync(ReadOnlyMemory<byte> data, CancellationToken cancellationToken = default);
    public abstract void StartListening(CancellationToken? cancellationToken = default);
}

public abstract class NamedPipeConnection<TPipeStream> : NamedPipeConnection
    where TPipeStream : PipeStream
{
    protected abstract Task ConnectAsync(TPipeStream pipeStream, TimeSpan timeout, CancellationToken cancellationToken);
    protected abstract TPipeStream CreatePipeStream();
    protected abstract void Disconnect(TPipeStream pipeStream);

    private readonly CancellationTokenSource _cancellationTokenSource;
    private readonly TimeSpan _closeTimeout;
    private readonly TimeSpan _openTimeout;
    private readonly IOperationHandler _operationHandler;
    private readonly IOperationInvoker _operationInvoker;
    private readonly SemaphoreSlim _readSemaphoreSlim;
    private readonly TimeSpan _receiveTimeout;
    private readonly TimeSpan _sendTimeout;
    private readonly SemaphoreSlim _writeSemaphoreSlim;
    private Task _connectionTask;
    private bool _isAtEndOfStream;
    private Task _listeningTask;
    private TPipeStream _pipeStream;

    protected NamedPipeConnection(
        IOperationInvoker operationInvoker,
        IOperationHandler operationHandler,
        TimeSpan openTimeout,
        TimeSpan closeTimeout,
        TimeSpan sendTimeout,
        TimeSpan receiveTimeout)
    {
        _openTimeout = openTimeout;
        _closeTimeout = closeTimeout;
        _sendTimeout = sendTimeout;
        _receiveTimeout = receiveTimeout;
        _operationInvoker = operationInvoker ?? throw new ArgumentNullException(nameof(operationInvoker));
        _operationHandler = operationHandler ?? throw new ArgumentNullException(nameof(operationHandler));

        _cancellationTokenSource = new CancellationTokenSource();
        _readSemaphoreSlim = new SemaphoreSlim(1);
        _writeSemaphoreSlim = new SemaphoreSlim(1);
    }

    ~NamedPipeConnection()
    {
        Dispose(false);
    }

    public override async Task HandleAsync(ReadOnlyMemory<byte> data, CancellationToken cancellationToken)
    {
        await _operationHandler.HandleAsync(this, data, cancellationToken);
    }

    public override object Invoke(object obj, MethodInfo targetMethod, object[] args)
    {
        return _operationInvoker.Invoke(this, targetMethod, args);
    }

    public override Task InvokeAsync(object obj, MethodInfo targetMethod, object[] args)
    {
        return _operationInvoker.InvokeAsync(this, targetMethod, args);
    }

    public override Task<T> InvokeAsync<T>(object obj, MethodInfo targetMethod, object[] args)
    {
        return _operationInvoker.InvokeAsync<T>(this, targetMethod, args);
    }

    public override async Task SendAsync(ReadOnlyMemory<byte> data, CancellationToken cancellationToken = default)
    {
        if (!_isAtEndOfStream && State is not CommunicationState.Opened)
        {
            throw new InvalidOperationException("Cannot send data.");
        }

        var cancellationTokenSource = CancellationTokenSource.CreateLinkedTokenSource(_cancellationTokenSource.Token, cancellationToken);
        cancellationToken = cancellationTokenSource.Token;
        cancellationToken.ThrowIfCancellationRequested();

        var result = false;
        try
        {
            result = await _writeSemaphoreSlim.WaitAsync(_sendTimeout, cancellationToken);

            if (!result)
            {
                throw new TimeoutException("The operation timed out.");
            }

            await WriteAsync(data, cancellationToken);
        }
        catch (NamedPipeException ex)
        {
            Fault(ex);
        }
        catch (IOException ex)
        {
            Fault(ex);
        }
        finally
        {
            if (result)
            {
                _writeSemaphoreSlim.Release();
            }
        }
    }

    public override void StartListening(CancellationToken? cancellationToken = default)
    {
        async Task InternalStartListeningAsync()
        {
            try
            {
                while (_cancellationTokenSource.IsCancellationRequested)
                {
                    await ListenAsync(_cancellationTokenSource.Token);

                    if (_isAtEndOfStream)
                    {
                        Disconnect();
                        break;
                    }
                }
            }
            catch (OperationCanceledException)
            {
                // Ignore
            }
            catch (Exception ex)
            {
                Fault(ex);
            }
        }

        _pipeStream.ReadMode = PipeTransmissionMode.Message;
        _listeningTask = Task.Run(InternalStartListeningAsync, cancellationToken ?? _cancellationTokenSource.Token);
    }

    protected override void OnDisconnecting()
    {
        Disconnect(_pipeStream);
        base.OnDisconnecting();
    }

    protected override void OnOpening()
    {
        _pipeStream = CreatePipeStream();
        _connectionTask = ConnectAsync(_pipeStream, _openTimeout, _cancellationTokenSource.Token);
        base.OnOpening();
    }

    private static bool IsEndOfStream(IReadOnlyList<byte> buffer)
    {
        return buffer.Count == 1 && buffer[0] == EndOfStream[0];
    }

    private async Task InternalCloseAsync(CancellationToken cancellationToken)
    {
        await WaitAllAsync(cancellationToken);

        _pipeStream.Close();
    }

    private async Task ListenAsync(CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var result = false;

        try
        {
            result = await _readSemaphoreSlim.WaitAsync(_sendTimeout, cancellationToken);

            if (!result)
            {
                throw new TimeoutException("The read operation timed out.");
            }

            await ReadAsync(cancellationToken);
        }
        finally
        {
            if (result)
            {
                _readSemaphoreSlim.Release();
            }
        }
    }

    private async Task ReadAsync(CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var buffer = new byte[_pipeStream.InBufferSize];
        var totalReadBytesCount = 0;
        int readBytesCount;

        do
        {
            cancellationToken.ThrowIfCancellationRequested();

            var timeoutCancellationToken = new CancellationTokenSource(_receiveTimeout);
            var cancellationTokenSource = CancellationTokenSource.CreateLinkedTokenSource(timeoutCancellationToken.Token, cancellationToken);
            readBytesCount = await _pipeStream.ReadAsync(buffer.AsMemory(totalReadBytesCount, buffer.Length - totalReadBytesCount), cancellationTokenSource.Token);
            totalReadBytesCount += readBytesCount;

            if (readBytesCount == _pipeStream.InBufferSize && !_pipeStream.IsMessageComplete)
            {
                Array.Resize(ref buffer, buffer.Length + _pipeStream.InBufferSize);
            }
        } while (readBytesCount > 0 && !_pipeStream.IsMessageComplete);

        if (totalReadBytesCount != 0)
        {
            if (IsEndOfStream(buffer))
            {
                _isAtEndOfStream = true;
                await InternalCloseAsync(cancellationToken);
            }

            // Don't block the listening loop with message processing
            _ = Task.Run(() => HandleAsync(buffer, cancellationToken), cancellationToken);
        }
        else
        {
            throw new NamedPipeException("Pipe is broken.");
        }
    }

    private void RequestTasksCancellation()
    {
        try
        {
            _cancellationTokenSource.Cancel();
        }
        catch (AggregateException)
        {
            // Ignore
        }
    }

    private async Task WaitAllAsync(CancellationToken cancellationToken)
    {
        RequestTasksCancellation();

        using var internalCancellationTokenSource = CancellationTokenSource.CreateLinkedTokenSource(_cancellationTokenSource.Token, cancellationToken);

        await WaitConnectionTaskAsync(internalCancellationTokenSource.Token);
        await WaitListeningTask(internalCancellationTokenSource.Token);
        await WaitReadWriteOperationsAsync(internalCancellationTokenSource.Token);
    }

    private async Task WaitConnectionTaskAsync(CancellationToken cancellationToken)
    {
        if (_connectionTask is null) return;

        if (_connectionTask.Status is not TaskStatus.WaitingForActivation && !_connectionTask.IsCompleted)
        {
            try
            {
                await _connectionTask.WaitAsync(cancellationToken);
            }
            catch (OperationCanceledException)
            {
                // Ignore
            }
        }
    }

    private async Task WaitListeningTask(CancellationToken cancellationToken)
    {
        if (_listeningTask is null) return;

        if (_listeningTask.Status is not TaskStatus.WaitingForActivation && !_listeningTask.IsCompleted)
        {
            try
            {
                await _listeningTask.WaitAsync(cancellationToken);
            }
            catch (OperationCanceledException)
            {
                // Ignore
            }
        }
    }

    private async Task WaitReadWriteOperationsAsync(CancellationToken cancellationToken)
    {
        try
        {
            await _readSemaphoreSlim.WaitAsync(cancellationToken);
            await _writeSemaphoreSlim.WaitAsync(cancellationToken);
        }
        catch (OperationCanceledException)
        {
            // Ignore
        }
    }

    private async Task WriteAsync(ReadOnlyMemory<byte> data, CancellationToken cancellationToken)
    {
        async Task InternalWriteAsync()
        {
            await _pipeStream.WriteAsync(data, cancellationToken);
            await _pipeStream.FlushAsync(cancellationToken);
        }

        cancellationToken.ThrowIfCancellationRequested();

        var timeoutCancellationToken = new CancellationTokenSource(_sendTimeout);
        var cancellationTokenSource = CancellationTokenSource.CreateLinkedTokenSource(timeoutCancellationToken.Token, cancellationToken);
        await Task.Run(InternalWriteAsync, cancellationTokenSource.Token);
    }
}