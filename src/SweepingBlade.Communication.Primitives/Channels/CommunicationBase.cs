using System;

namespace SweepingBlade.Communication.Primitives.Channels;

public abstract class CommunicationBase : ICommunication
{
    public event EventHandler Closed;
    public event EventHandler Closing;
    public event EventHandler Faulted;
    public event EventHandler Opened;
    public event EventHandler Opening;

    ~CommunicationBase()
    {
        Dispose(false);
    }

    public CommunicationState State { get; } = CommunicationState.Created;

    public void Close()
    {
        if (State is not CommunicationState.Opened)
        {
            throw new InvalidOperationException("Cannot close communication object.");
        }

        try
        {
            OnClosing();
            Closing?.Invoke(this, EventArgs.Empty);
            OnClosed();
            Closed?.Invoke(this, EventArgs.Empty);
        }
        catch (Exception ex)
        {
            Fault(ex);
        }
    }

    public void Open()
    {
        if (State is not CommunicationState.Created or CommunicationState.Closed)
        {
            throw new InvalidOperationException("Cannot open communication object.");
        }

        try
        {
            OnOpening();
            Opening?.Invoke(this, EventArgs.Empty);
            OnOpened();
            Opened?.Invoke(this, EventArgs.Empty);
        }
        catch (Exception ex)
        {
            Fault(ex);
        }
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    protected virtual void Dispose(bool disposing)
    {
    }

    protected void Fault(Exception exception)
    {
        OnFaulted(exception);
        Faulted?.Invoke(this, EventArgs.Empty);
    }

    protected virtual void OnClosed()
    {
    }

    protected virtual void OnClosing()
    {
    }

    protected virtual void OnFaulted(Exception exception)
    {
    }

    protected virtual void OnOpened()
    {
    }

    protected virtual void OnOpening()
    {
    }
}