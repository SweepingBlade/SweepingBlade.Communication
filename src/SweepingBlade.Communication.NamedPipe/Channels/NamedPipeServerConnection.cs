using System;
using System.ComponentModel;
using System.IO.Pipes;
using System.Threading;
using System.Threading.Tasks;
using SweepingBlade.Communication.Primitives;
using SweepingBlade.Communication.Primitives.Dispatcher;

namespace SweepingBlade.Communication.NamedPipe.Channels;

public class NamedPipeServerConnection : NamedPipeConnection<NamedPipeServerStream>
{
    private readonly EndpointAddress _endpointAddress;
    private readonly int _inputBufferSize;
    private readonly int _maxConnections;
    private readonly int _outputBufferSize;
    private readonly PipeDirection _pipeDirection;
    private readonly PipeOptions _pipeOptions;
    private readonly PipeTransmissionMode _pipeTransmissionMode;

    public NamedPipeServerConnection(
        IOperationInvoker operationInvoker,
        IOperationHandler operationHandler,
        EndpointAddress endpointAddress,
        PipeDirection pipeDirection,
        PipeOptions pipeOptions,
        PipeTransmissionMode pipeTransmissionMode,
        int maxConnections,
        int inputBufferSize,
        int outputBufferSize,
        TimeSpan openTimeout,
        TimeSpan closeTimeout,
        TimeSpan sendTimeout,
        TimeSpan receiveTimeout)
        : base(
            operationInvoker,
            operationHandler,
            openTimeout,
            closeTimeout,
            sendTimeout,
            receiveTimeout)
    {
        if (!Enum.IsDefined(typeof(PipeDirection), pipeDirection)) throw new InvalidEnumArgumentException(nameof(pipeDirection), (int)pipeDirection, typeof(PipeDirection));
        if (!Enum.IsDefined(typeof(PipeOptions), pipeOptions)) throw new InvalidEnumArgumentException(nameof(pipeOptions), (int)pipeOptions, typeof(PipeOptions));
        if (!Enum.IsDefined(typeof(PipeTransmissionMode), pipeTransmissionMode)) throw new InvalidEnumArgumentException(nameof(pipeTransmissionMode), (int)pipeTransmissionMode, typeof(PipeTransmissionMode));

        _endpointAddress = endpointAddress ?? throw new ArgumentNullException(nameof(endpointAddress));
        _pipeDirection = pipeDirection;
        _pipeOptions = pipeOptions;
        _pipeTransmissionMode = pipeTransmissionMode;
        _maxConnections = maxConnections;
        _inputBufferSize = inputBufferSize;
        _outputBufferSize = outputBufferSize;
    }

    protected override Task ConnectAsync(NamedPipeServerStream pipeStream, TimeSpan timeout, CancellationToken cancellationToken)
    {
        async Task InternalConnectAsync()
        {
            try
            {
                await pipeStream.WaitForConnectionAsync(cancellationToken);
                Connect();
                StartListening();
            }
            catch (OperationCanceledException)
            {
                throw;
            }
            catch (Exception ex)
            {
                Fault(ex);
            }
        }

        return Task.Run(InternalConnectAsync, cancellationToken);
    }

    protected override NamedPipeServerStream CreatePipeStream()
    {
        return new NamedPipeServerStream(
            _endpointAddress.ToNamedPipe(),
            _pipeDirection,
            _maxConnections,
            _pipeTransmissionMode,
            _pipeOptions,
            _inputBufferSize,
            _outputBufferSize);
    }

    protected override void Disconnect(NamedPipeServerStream pipeStream)
    {
        pipeStream.Disconnect();
    }
}