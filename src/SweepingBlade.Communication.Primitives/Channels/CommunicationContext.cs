using System;

namespace SweepingBlade.Communication.Primitives.Channels;

public abstract class CommunicationContext : CommunicationBase, ICommunicationContext
{
    public event EventHandler Connected;
    public event EventHandler Connecting;
    public event EventHandler Disconnected;
    public event EventHandler Disconnecting;

    protected void Connect()
    {
        OnConnecting();
        Connecting?.Invoke(this, EventArgs.Empty);
        OnConnected();
        Connected?.Invoke(this, EventArgs.Empty);
    }

    protected void Disconnect()
    {
        OnDisconnecting();
        Disconnecting?.Invoke(this, EventArgs.Empty);
        OnDisconnected();
        Disconnected?.Invoke(this, EventArgs.Empty);
    }

    protected virtual void OnConnected()
    {
    }

    protected virtual void OnConnecting()
    {
    }

    protected virtual void OnDisconnected()
    {
    }

    protected virtual void OnDisconnecting()
    {
    }
}