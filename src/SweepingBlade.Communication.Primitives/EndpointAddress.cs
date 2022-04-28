using System;

namespace SweepingBlade.Communication.Primitives;

public class EndpointAddress
{
    public Uri Uri { get; }

    public EndpointAddress(Uri uri)
    {
        Uri = uri ?? throw new ArgumentNullException(nameof(uri));
    }
}