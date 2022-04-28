using System;
using SweepingBlade.Communication.Primitives.Dispatcher;

namespace SweepingBlade.Communication.Primitives.Channels;

public class ClientChannel<TService> : CommunicationContext, IClientChannel<TService>
    where TService : class
{
    private readonly IConnectionFactory _connectionFactory;
    private readonly EndpointAddress _endpointAddress;
    private readonly IOperationHandler _operationHandler;
    private readonly IOperationInvoker _operationInvoker;
    private IConnection _connection;
    private TService _proxy;

    public ClientChannel(
        IConnectionFactory connectionFactory,
        IOperationHandler operationHandler,
        IOperationInvoker operationInvoker,
        EndpointAddress endpointAddress)
    {
        _connectionFactory = connectionFactory ?? throw new ArgumentNullException(nameof(connectionFactory));
        _operationHandler = operationHandler ?? throw new ArgumentNullException(nameof(operationHandler));
        _operationInvoker = operationInvoker ?? throw new ArgumentNullException(nameof(operationInvoker));
        _endpointAddress = endpointAddress ?? throw new ArgumentNullException(nameof(endpointAddress));
    }

    public TService CreateProxy()
    {
        return _proxy ??= DispatchProxy<TService>.Create(_connection);
    }

    protected override void OnOpening()
    {
        _connection = _connectionFactory.CreateClientConnection(_operationInvoker, _operationHandler, _endpointAddress);
        _connection.Open();
        base.OnOpening();
    }
}