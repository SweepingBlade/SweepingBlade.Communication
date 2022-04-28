using System;

namespace SweepingBlade.Communication.Primitives;

public sealed class Endpoint
{
    public EndpointAddress Address { get; }
    public EndpointAddress CallbackAddress { get; }

    public Endpoint(EndpointAddress address, EndpointAddress callbackAddress = null)
    {
        Address = address ?? throw new ArgumentNullException(nameof(address));
        CallbackAddress = callbackAddress;
    }
}