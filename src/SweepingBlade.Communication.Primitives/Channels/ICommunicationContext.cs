using System;

namespace SweepingBlade.Communication.Primitives.Channels;

public interface ICommunicationContext : ICommunication
{
    event EventHandler Connected;
    event EventHandler Connecting;
    event EventHandler Disconnected;
    event EventHandler Disconnecting;
}