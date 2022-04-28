using SweepingBlade.Communication.Primitives.Description;

namespace SweepingBlade.Communication.Primitives.Channels;

public interface IDuplexServiceChannel<TService, out TServiceCallback> : IServiceChannel<TService>
    where TService : class
    where TServiceCallback : class
{
    IClientChannel<TServiceCallback> CreateClientChannel(ServiceDescription serviceDescription, ServiceEndpoint serviceEndpoint, EndpointAddress endpointAddress);
}