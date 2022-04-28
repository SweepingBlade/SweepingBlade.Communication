using System;
using SweepingBlade.Communication.Primitives.Dispatcher;

namespace SweepingBlade.Communication.Primitives.Channels;

public class DuplexClientChannel<TService, TServiceCallback> : ClientChannel<TService>, IDuplexClientChannel<TService, TServiceCallback>
    where TService : class
    where TServiceCallback : class
{
    public event EventHandler ClientConnected;
    public event EventHandler ClientConnecting;
    public event EventHandler ClientDisconnected;
    public event EventHandler ClientDisconnecting;

    public DuplexClientChannel(
        IConnectionFactory connectionFactory,
        IOperationHandler operationHandler,
        IOperationInvoker operationInvoker,
        EndpointAddress endpointAddress)
        : base(
            connectionFactory,
            operationHandler,
            operationInvoker,
            endpointAddress)
    {
    }

    protected void ConnectClient()
    {
        OnClientConnecting();
        ClientConnecting?.Invoke(this, EventArgs.Empty);
        OnClientConnected();
        ClientConnected?.Invoke(this, EventArgs.Empty);
    }

    protected void DisconnectClient()
    {
        OnClientDisconnecting();
        ClientDisconnecting?.Invoke(this, EventArgs.Empty);
        OnClientDisconnected();
        ClientDisconnected?.Invoke(this, EventArgs.Empty);
    }

    protected virtual void OnClientConnected()
    {
    }

    protected virtual void OnClientConnecting()
    {
    }

    protected virtual void OnClientDisconnected()
    {
    }

    protected virtual void OnClientDisconnecting()
    {
    }
}