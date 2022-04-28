namespace SweepingBlade.Communication.Primitives.Channels;

public enum CommunicationState
{
    Created,
    Opening,
    Opened,
    Closing,
    Closed,
    Faulted
}