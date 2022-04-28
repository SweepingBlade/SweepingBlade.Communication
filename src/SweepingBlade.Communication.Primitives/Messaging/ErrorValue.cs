using System;

namespace SweepingBlade.Communication.Primitives.Messaging;

public sealed class ErrorValue
{
    public FaultException Exception { get; }

    public ErrorValue(FaultException exception)
    {
        Exception = exception ?? throw new ArgumentNullException(nameof(exception));
    }
}