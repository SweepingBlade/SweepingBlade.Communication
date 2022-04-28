using System;

namespace SweepingBlade.Communication.Primitives.Messaging;

public sealed class OperationInvocationResponseMessage : OperationInvocationEndpointMessage
{
    public OperationInvocationResponse Response { get; }

    public OperationInvocationResponseMessage(Endpoint endpoint, OperationInvocationResponse response)
        : base(endpoint)
    {
        Response = response ?? throw new ArgumentNullException(nameof(response));
    }
}