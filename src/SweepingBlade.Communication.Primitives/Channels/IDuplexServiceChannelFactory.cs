using SweepingBlade.Communication.Primitives.Description;

namespace SweepingBlade.Communication.Primitives.Channels;

public interface IDuplexServiceChannelFactory
{
    IDuplexServiceChannel<TService, TServiceCallback> CreateDuplexServiceChannel<TService, TServiceCallback>(
        ServiceDescription serviceDescription,
        ServiceEndpoint serviceEndpoint,
        EndpointAddress endpointAddress)
        where TService : class
        where TServiceCallback : class;
}