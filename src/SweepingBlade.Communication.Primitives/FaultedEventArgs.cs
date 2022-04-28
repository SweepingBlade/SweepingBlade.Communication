using System;

namespace SweepingBlade.Communication.Primitives;

public class FaultedEventArgs : EventArgs
{
    public Exception Exception { get; }

    public FaultedEventArgs(Exception exception)
    {
        Exception = exception ?? throw new ArgumentNullException(nameof(exception));
    }
}