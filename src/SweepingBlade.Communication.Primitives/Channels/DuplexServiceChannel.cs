using System;
using SweepingBlade.Communication.Primitives.Description;
using SweepingBlade.Communication.Primitives.Dispatcher;

namespace SweepingBlade.Communication.Primitives.Channels;

public class DuplexServiceChannel<TService, TServiceCallback> : ServiceChannel, IDuplexServiceChannel<TService, TServiceCallback>
    where TService : class
    where TServiceCallback : class
{
    private readonly IClientChannelFactory _clientChannelFactory;

    public DuplexServiceChannel(
        IClientChannelFactory clientChannelFactory,
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
        _clientChannelFactory = clientChannelFactory ?? throw new ArgumentNullException(nameof(clientChannelFactory));
    }

    public IClientChannel<TServiceCallback> CreateClientChannel(ServiceDescription serviceDescription, ServiceEndpoint serviceEndpoint, EndpointAddress endpointAddress)
    {
        return _clientChannelFactory.CreateClientChannel<TServiceCallback>(serviceDescription, serviceEndpoint, endpointAddress);
    }
}