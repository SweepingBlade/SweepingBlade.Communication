namespace SweepingBlade.Communication.Primitives.Channels;

public interface IDuplexClientChannel<out TService, TServiceCallback> : IClientChannel<TService>, IServiceChannel<TServiceCallback>
    where TService : class
    where TServiceCallback : class
{
}