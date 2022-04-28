namespace SweepingBlade.Communication.Primitives.Channels;

public interface IClientChannel : ICommunicationContext
{
}

public interface IClientChannel<out TService> : IClientChannel
    where TService : class
{
    TService CreateProxy();
}