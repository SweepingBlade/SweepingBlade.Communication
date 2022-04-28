using System;

namespace SweepingBlade.Communication.Primitives.Channels;

public abstract class ServiceChannelContext : CommunicationContext, IServiceChannelContext
{
    public event EventHandler ClientConnected;
    public event EventHandler ClientConnecting;
    public event EventHandler ClientDisconnected;
    public event EventHandler ClientDisconnecting;

    protected void ConnectClient(IConnection connection)
    {
        OnClientConnecting(connection);
        ClientConnecting?.Invoke(this, EventArgs.Empty);
        OnClientConnected(connection);
        ClientConnected?.Invoke(this, EventArgs.Empty);
    }

    protected void DisconnectClient(IConnection connection)
    {
        OnClientDisconnecting(connection);
        ClientDisconnecting?.Invoke(this, EventArgs.Empty);
        OnClientDisconnected(connection);
        ClientDisconnected?.Invoke(this, EventArgs.Empty);
    }

    protected virtual void OnClientConnected(IConnection connection)
    {
    }

    protected virtual void OnClientConnecting(IConnection connection)
    {
    }

    protected virtual void OnClientDisconnected(IConnection connection)
    {
    }

    protected virtual void OnClientDisconnecting(IConnection connection)
    {
    }
}