using SweepingBlade.Communication.Primitives.Description;

namespace SweepingBlade.Communication.Primitives.Channels;

public interface IDuplexClientChannelFactory
{
    IDuplexClientChannel<TService, TServiceCallback> CreateDuplexClientChannel<TService, TServiceCallback>(
        ServiceDescription serviceDescription,
        ServiceEndpoint serviceEndpoint,
        EndpointAddress endpointAddress)
        where TService : class
        where TServiceCallback : class;
}