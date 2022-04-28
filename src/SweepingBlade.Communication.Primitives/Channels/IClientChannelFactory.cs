using SweepingBlade.Communication.Primitives.Description;

namespace SweepingBlade.Communication.Primitives.Channels;

public interface IClientChannelFactory
{
    public IClientChannel<TService> CreateClientChannel<TService>(
        ServiceDescription serviceDescription,
        ServiceEndpoint serviceEndpoint,
        EndpointAddress endpointAddress)
        where TService : class;
}