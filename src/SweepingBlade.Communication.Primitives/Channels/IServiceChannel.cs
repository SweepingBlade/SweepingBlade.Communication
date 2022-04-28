namespace SweepingBlade.Communication.Primitives.Channels;

public interface IServiceChannel : IServiceChannelContext
{
}

public interface IServiceChannel<TService> : IServiceChannel
    where TService : class
{
}