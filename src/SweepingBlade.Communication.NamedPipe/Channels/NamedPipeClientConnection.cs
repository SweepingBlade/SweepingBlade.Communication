using System;
using System.ComponentModel;
using System.IO;
using System.IO.Pipes;
using System.Security.Principal;
using System.Threading;
using System.Threading.Tasks;
using SweepingBlade.Communication.Primitives;
using SweepingBlade.Communication.Primitives.Dispatcher;

namespace SweepingBlade.Communication.NamedPipe.Channels;

public class NamedPipeClientConnection : NamedPipeConnection<NamedPipeClientStream>
{
    private readonly EndpointAddress _endpointAddress;
    private readonly HandleInheritability _handleInheritability;
    private readonly PipeDirection _pipeDirection;
    private readonly PipeOptions _pipeOptions;
    private readonly TokenImpersonationLevel _tokenImpersonationLevel;

    public NamedPipeClientConnection(
        IOperationInvoker operationInvoker,
        IOperationHandler operationHandler,
        EndpointAddress endpointAddress,
        PipeDirection pipeDirection,
        PipeOptions pipeOptions,
        TokenImpersonationLevel tokenImpersonationLevel,
        HandleInheritability handleInheritability,
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
        if (!Enum.IsDefined(typeof(TokenImpersonationLevel), tokenImpersonationLevel)) throw new InvalidEnumArgumentException(nameof(tokenImpersonationLevel), (int)tokenImpersonationLevel, typeof(TokenImpersonationLevel));
        if (!Enum.IsDefined(typeof(HandleInheritability), handleInheritability)) throw new InvalidEnumArgumentException(nameof(handleInheritability), (int)handleInheritability, typeof(HandleInheritability));

        _endpointAddress = endpointAddress ?? throw new ArgumentNullException(nameof(endpointAddress));
        _pipeDirection = pipeDirection;
        _pipeOptions = pipeOptions;
        _tokenImpersonationLevel = tokenImpersonationLevel;
        _handleInheritability = handleInheritability;
    }

    protected override Task ConnectAsync(NamedPipeClientStream pipeStream, TimeSpan timeout, CancellationToken cancellationToken)
    {
        async Task InternalConnectAsync()
        {
            try
            {
                var timeoutCancellationToken = new CancellationTokenSource(timeout);
                var cancellationTokenSource = CancellationTokenSource.CreateLinkedTokenSource(timeoutCancellationToken.Token, cancellationToken);
                await pipeStream.ConnectAsync(cancellationTokenSource.Token);
                Connect();
                StartListening(cancellationToken);
            }
            catch (OperationCanceledException)
            {
                throw;
            }
            catch (TimeoutException)
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

    protected override NamedPipeClientStream CreatePipeStream()
    {
        return new NamedPipeClientStream(
            ".",
            _endpointAddress.ToNamedPipe(),
            _pipeDirection,
            _pipeOptions,
            _tokenImpersonationLevel,
            _handleInheritability);
    }

    protected override void Disconnect(NamedPipeClientStream pipeStream)
    {
        pipeStream.Close();
    }
}