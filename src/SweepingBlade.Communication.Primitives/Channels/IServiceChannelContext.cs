using System;

namespace SweepingBlade.Communication.Primitives.Channels;

public interface IServiceChannelContext : ICommunicationContext
{
    event EventHandler ClientConnected;
    event EventHandler ClientConnecting;
    event EventHandler ClientDisconnected;
    event EventHandler ClientDisconnecting;
}