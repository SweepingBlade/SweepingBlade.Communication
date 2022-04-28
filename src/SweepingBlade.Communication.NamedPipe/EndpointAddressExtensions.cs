using System;
using System.Text;
using SweepingBlade.Communication.Primitives;

namespace SweepingBlade.Communication.NamedPipe;

public static class EndpointAddressExtensions
{
    public static string ToNamedPipe(this EndpointAddress endpointAddress)
    {
        if (endpointAddress is null) throw new ArgumentNullException(nameof(endpointAddress));

        var uriString = endpointAddress.Uri.ToString();
        return Convert.ToBase64String(Encoding.UTF8.GetBytes(uriString));
    }
}