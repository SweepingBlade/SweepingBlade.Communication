using System;

namespace SweepingBlade.Communication.Primitives.Channels;

public interface ICommunication : IDisposable
{
    public event EventHandler Closed;
    public event EventHandler Closing;
    public event EventHandler Faulted;
    public event EventHandler Opened;
    public event EventHandler Opening;
    public void Close();
    public void Open();
    public CommunicationState State { get; }
}