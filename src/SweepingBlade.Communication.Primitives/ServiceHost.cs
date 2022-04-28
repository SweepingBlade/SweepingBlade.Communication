using System;
using System.Collections.Concurrent;
using SweepingBlade.Communication.Primitives.Behaviors;
using SweepingBlade.Communication.Primitives.Channels;

namespace SweepingBlade.Communication.Primitives;

public sealed class ServiceHost : CommunicationBase, IServiceHost
{
    private readonly ConcurrentBag<IServiceChannel> _serviceChannels;
    private bool _disposed;

    public ServiceHost()
    {
        _serviceChannels = new ConcurrentBag<IServiceChannel>();
    }

    ~ServiceHost()
    {
        Dispose(false);
    }

    protected override void Dispose(bool disposing)
    {
        if (!_disposed)
        {
            if (disposing)
            {
                foreach (var serviceChannel in _serviceChannels)
                {
                    serviceChannel.Dispose();
                }
            }

            _disposed = true;
        }

        base.Dispose(disposing);
    }

    protected override void OnClosing()
    {
        foreach (var serviceChannel in _serviceChannels)
        {
            serviceChannel.Close();
        }
    }

    protected override void OnOpening()
    {
        foreach (var serviceChannel in _serviceChannels)
        {
            serviceChannel.Open();
        }
    }

    public void AddServiceEndpoint<TService>(TService instance, Options options, Uri uri)
        where TService : class
    {
        if (instance is null) throw new ArgumentNullException(nameof(instance));
        if (options is null) throw new ArgumentNullException(nameof(options));

        var serviceType = typeof(TService);
        var description = options.DescriptionResolver.ResolveService(serviceType);
        var endpointAddress = uri.CreateEndpointAddress<TService>();
        var serviceEndpoint = options.DescriptionResolver.CreateServiceEndpoint(serviceType, endpointAddress);
        description.AddEndpoint(serviceEndpoint);

        var serviceChannel = options.ChannelFactory.CreateServiceChannel<TService>(description, serviceEndpoint, endpointAddress);
        serviceChannel.ApplyDefaultBehaviors<TService>(description, options.Serializer);

        _serviceChannels.Add(serviceChannel);
    }

    public void AddServiceEndpoint<TService, TServiceCallback>(TService instance, Options options, Uri uri)
        where TService : class
        where TServiceCallback : class
    {
        if (instance is null) throw new ArgumentNullException(nameof(instance));
        if (options is null) throw new ArgumentNullException(nameof(options));

        var serviceType = typeof(TService);
        var description = options.DescriptionResolver.ResolveService(serviceType);
        var endpointAddress = uri.CreateEndpointAddress<TService, TServiceCallback>();
        var serviceEndpoint = options.DescriptionResolver.CreateServiceEndpoint(serviceType, endpointAddress);
        description.AddEndpoint(serviceEndpoint);

        var serviceCallbackType = typeof(TServiceCallback);
        var callbackDescription = options.DescriptionResolver.ResolveService(serviceCallbackType);
        var callbackServiceEndpoint = options.DescriptionResolver.CreateServiceEndpoint(serviceCallbackType, endpointAddress);
        description.AddEndpoint(callbackServiceEndpoint);

        var serviceChannel = options.ChannelFactory.CreateDuplexServiceChannel<TService, TServiceCallback>(description, serviceEndpoint, endpointAddress);
        serviceChannel.ApplyDefaultBehaviors(description, callbackDescription, callbackServiceEndpoint, options.Serializer);

        _serviceChannels.Add(serviceChannel);
    }
}