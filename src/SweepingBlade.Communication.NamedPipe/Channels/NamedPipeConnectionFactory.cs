using System;
using System.ComponentModel;
using System.IO;
using System.IO.Pipes;
using System.Security.Principal;
using SweepingBlade.Communication.Primitives;
using SweepingBlade.Communication.Primitives.Channels;
using SweepingBlade.Communication.Primitives.Dispatcher;

namespace SweepingBlade.Communication.NamedPipe.Channels;

public class NamedPipeConnectionFactory : IConnectionFactory
{
    private const HandleInheritability HandleInheritability = System.IO.HandleInheritability.None;
    private const PipeDirection PipeDirection = System.IO.Pipes.PipeDirection.InOut;
    private const PipeTransmissionMode PipeTransmissionMode = System.IO.Pipes.PipeTransmissionMode.Message;

    private readonly TimeSpan _closeTimeout;
    private readonly int _inputBufferSize;
    private readonly int _maxConnections;
    private readonly TimeSpan _openTimeout;
    private readonly int _outputBufferSize;
    private readonly TimeSpan _receiveTimeout;
    private readonly NamedPipeSecurityMode _securityMode;
    private readonly TimeSpan _sendTimeout;

    private PipeOptions? _pipeOptions;
    private TokenImpersonationLevel? _tokenImpersonationLevel;

    private PipeOptions PipeOptions => _pipeOptions ??= CreatePipeOptions();
    private TokenImpersonationLevel TokenImpersonationLevel => _tokenImpersonationLevel ??= CreateTokenImpersonationLevel();

    public NamedPipeConnectionFactory(
        NamedPipeSecurityMode securityMode,
        TimeSpan openTimeout,
        TimeSpan closeTimeout,
        TimeSpan sendTimeout,
        TimeSpan receiveTimeout,
        int maxConnections,
        int inputBufferSize,
        int outputBufferSize)
    {
        if (!Enum.IsDefined(typeof(NamedPipeSecurityMode), securityMode)) throw new InvalidEnumArgumentException(nameof(securityMode), (int)securityMode, typeof(NamedPipeSecurityMode));

        _openTimeout = openTimeout;
        _closeTimeout = closeTimeout;
        _sendTimeout = sendTimeout;
        _receiveTimeout = receiveTimeout;
        _maxConnections = maxConnections;
        _inputBufferSize = inputBufferSize;
        _outputBufferSize = outputBufferSize;

        _securityMode = securityMode;
    }

    public IConnection CreateClientConnection(IOperationInvoker operationInvoker, IOperationHandler operationHandler, EndpointAddress endpointAddress)
    {
        return new NamedPipeClientConnection(
            operationInvoker,
            operationHandler,
            endpointAddress,
            PipeDirection,
            PipeOptions,
            TokenImpersonationLevel,
            HandleInheritability,
            _openTimeout,
            _closeTimeout,
            _sendTimeout,
            _receiveTimeout);
    }

    public IConnection CreateServerConnection(IOperationInvoker operationInvoker, IOperationHandler operationHandler, EndpointAddress endpointAddress)
    {
        return new NamedPipeServerConnection(
            operationInvoker,
            operationHandler,
            endpointAddress,
            PipeDirection,
            PipeOptions,
            PipeTransmissionMode,
            _maxConnections,
            _inputBufferSize,
            _outputBufferSize,
            _openTimeout,
            _closeTimeout,
            _sendTimeout,
            _receiveTimeout);
    }

    private PipeOptions CreatePipeOptions()
    {
        var pipeOptions = PipeOptions.Asynchronous;

        if (_securityMode is NamedPipeSecurityMode.Transport)
        {
            pipeOptions |= PipeOptions.CurrentUserOnly;
        }

        return pipeOptions;
    }

    private TokenImpersonationLevel CreateTokenImpersonationLevel()
    {
        var tokenImpersonationLevel = TokenImpersonationLevel.None;

        if (_securityMode is NamedPipeSecurityMode.Transport)
        {
            tokenImpersonationLevel = TokenImpersonationLevel.Impersonation;
        }

        return tokenImpersonationLevel;
    }
}