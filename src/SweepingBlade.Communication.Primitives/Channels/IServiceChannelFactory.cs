using SweepingBlade.Communication.Primitives.Description;

namespace SweepingBlade.Communication.Primitives.Channels;

public interface IServiceChannelFactory
{
    IServiceChannel<TService> CreateServiceChannel<TService>(
        ServiceDescription serviceDescription,
        ServiceEndpoint serviceEndpoint,
        EndpointAddress endpointAddress)
        where TService : class;
}