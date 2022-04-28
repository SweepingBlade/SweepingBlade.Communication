using System;

namespace SweepingBlade.Communication.Primitives.Messaging;

public sealed class EndpointMessage
{
    public Guid Id { get; }
    public Message Message { get; }

    public EndpointMessage(Guid id, Message message)
    {
        Id = id;
        Message = message ?? throw new ArgumentNullException(nameof(message));
    }
}