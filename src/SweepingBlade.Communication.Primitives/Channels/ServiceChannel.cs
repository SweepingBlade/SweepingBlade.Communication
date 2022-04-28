using System;
using System.Collections.Concurrent;
using SweepingBlade.Communication.Primitives.Dispatcher;

namespace SweepingBlade.Communication.Primitives.Channels;

public class ServiceChannel : ServiceChannelContext, IServiceChannel
{
    private readonly IConnectionFactory _connectionFactory;
    private readonly ConcurrentBag<IConnection> _connections;
    private readonly EndpointAddress _endpointAddress;
    private readonly IOperationHandler _operationHandler;
    private readonly IOperationInvoker _operationInvoker;

    protected ServiceChannel(
        IConnectionFactory connectionFactory,
        IOperationInvoker operationInvoker,
        IOperationHandler operationHandler,
        EndpointAddress endpointAddress)
    {
        _connectionFactory = connectionFactory ?? throw new ArgumentNullException(nameof(connectionFactory));
        _operationInvoker = operationInvoker ?? throw new ArgumentNullException(nameof(operationInvoker));
        _operationHandler = operationHandler ?? throw new ArgumentNullException(nameof(operationHandler));
        _endpointAddress = endpointAddress ?? throw new ArgumentNullException(nameof(endpointAddress));

        _connections = new ConcurrentBag<IConnection>();
    }

    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            foreach (var connection in _connections)
            {
                connection.Connected -= ConnectionOnConnected;
                connection.Dispose();
            }
        }

        base.Dispose(disposing);
    }

    protected override void OnClientConnecting(IConnection connection)
    {
        SpinUpConnection();
        base.OnClientConnecting(connection);
    }

    protected override void OnOpening()
    {
        SpinUpConnection();
        base.OnOpening();
    }

    private void SpinUpConnection()
    {
        var connection = _connectionFactory.CreateServerConnection(_operationInvoker, _operationHandler, _endpointAddress);
        connection.Connected += ConnectionOnConnected;
        _connections.Add(connection);
    }

    private void ConnectionOnConnected(object sender, EventArgs e)
    {
        ConnectClient((IConnection)sender);
    }
}

public class ServiceChannel<TService> : ServiceChannel, IServiceChannel<TService>
    where TService : class
{
    public ServiceChannel(
        IConnectionFactory connectionFactory,
        IOperationInvoker operationInvoker,
        IOperationHandler operationHandler,
        EndpointAddress endpointAddress)
        : base(
            connectionFactory,
            operationInvoker,
            operationHandler,
            endpointAddress)
    {
    }
}