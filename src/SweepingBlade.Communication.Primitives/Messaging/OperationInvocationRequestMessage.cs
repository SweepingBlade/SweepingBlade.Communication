using System;

namespace SweepingBlade.Communication.Primitives.Messaging;

public sealed class OperationInvocationRequestMessage : OperationInvocationEndpointMessage
{
    public OperationInvocationRequest Request { get; }

    public OperationInvocationRequestMessage(Endpoint endpoint, OperationInvocationRequest request)
        : base(endpoint)
    {
        Request = request ?? throw new ArgumentNullException(nameof(request));
    }
}