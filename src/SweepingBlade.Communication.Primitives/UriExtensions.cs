using System;

namespace SweepingBlade.Communication.Primitives;

internal static class UriExtensions
{
    private const char BackSlash = '\\';
    private const char ForwardSlash = '/';

    public static EndpointAddress CreateEndpointAddress<TService>(this Uri uri)
        where TService : class
    {
        var endpointUri = uri.Join(typeof(TService).Name);
        return new EndpointAddress(endpointUri);
    }

    public static EndpointAddress CreateEndpointAddress<TService, TServiceCallback>(this Uri uri)
        where TService : class
        where TServiceCallback : class
    {
        var endpointUri = uri.Join(typeof(TService).Name)
            .Join(typeof(TServiceCallback).Name);
        return new EndpointAddress(endpointUri);
    }

    public static Uri Join(this Uri uri, string component)
    {
        if (uri is null) throw new ArgumentNullException(nameof(uri));
        if (component is null || component.Length == 0)
        {
            return uri;
        }

        component = component.TrimEnd(ForwardSlash, BackSlash);
        component = component.TrimStart(ForwardSlash, BackSlash);

        return new Uri(uri, component);
    }
}