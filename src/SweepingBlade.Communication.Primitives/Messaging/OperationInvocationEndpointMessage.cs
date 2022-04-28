using System;

namespace SweepingBlade.Communication.Primitives.Messaging;

public abstract class OperationInvocationEndpointMessage : Message
{
    public Endpoint Endpoint { get; }

    protected OperationInvocationEndpointMessage(Endpoint endpoint)
    {
        Endpoint = endpoint ?? throw new ArgumentNullException(nameof(endpoint));
    }
}