using System;

namespace SweepingBlade.Communication.Primitives.Channels;

public interface IChannelFactory :
    IClientChannelFactory,
    IDuplexClientChannelFactory,
    IDuplexServiceChannelFactory,
    IServiceChannelFactory
{
    IClientChannel<TService> CreateClientChannel<TService>(Uri uri)
        where TService : class;
}