using System;
using SweepingBlade.Communication.Primitives;
using SweepingBlade.Communication.Primitives.Channels;
using SweepingBlade.Communication.Primitives.Description;
using SweepingBlade.Communication.Primitives.Transports;

namespace SweepingBlade.Communication.NamedPipe.Channels;

public class NamedPipeChannelFactory : IChannelFactory
{
    private readonly NamedPipeOptions _options;

    public NamedPipeChannelFactory(NamedPipeOptions options)
    {
        _options = options ?? throw new ArgumentNullException(nameof(options));
    }

    public IClientChannel<TService> CreateClientChannel<TService>(Uri uri)
        where TService : class
    {
        if (uri is null) throw new ArgumentNullException(nameof(uri));
        var serviceType = typeof(TService);
        var endpointAddress = new EndpointAddress(uri);
        var serviceEndpoint = _options.DescriptionResolver.CreateServiceEndpoint(serviceType, endpointAddress);
        var serviceDescription = _options.DescriptionResolver.ResolveService(serviceType);
        return CreateClientChannel<TService>(serviceDescription, serviceEndpoint, endpointAddress);
    }

    public IClientChannel<TService> CreateClientChannel<TService>(
        ServiceDescription serviceDescription,
        ServiceEndpoint serviceEndpoint,
        EndpointAddress endpointAddress)
        where TService : class
    {
        var endpoint = new Endpoint(endpointAddress);
        var transport = new RequestReplyTransport(
            _options.Serializer,
            serviceDescription,
            serviceEndpoint,
            endpoint);
        return new ClientChannel<TService>(
            _options.ConnectionFactory,
            transport,
            transport,
            endpointAddress);
    }

    public IDuplexClientChannel<TService, TServiceCallback> CreateDuplexClientChannel<TService, TServiceCallback>(
        ServiceDescription serviceDescription,
        ServiceEndpoint serviceEndpoint,
        EndpointAddress endpointAddress)
        where TService : class
        where TServiceCallback : class
    {
        var endpoint = new Endpoint(endpointAddress);
        var transport = new RequestReplyTransport(
            _options.Serializer,
            serviceDescription,
            serviceEndpoint,
            endpoint);
        return new DuplexClientChannel<TService, TServiceCallback>(
            _options.ConnectionFactory,
            transport,
            transport,
            endpointAddress);
    }

    public IDuplexServiceChannel<TService, TServiceCallback> CreateDuplexServiceChannel<TService, TServiceCallback>(
        ServiceDescription serviceDescription,
        ServiceEndpoint serviceEndpoint,
        EndpointAddress endpointAddress)
        where TService : class
        where TServiceCallback : class
    {
        var endpoint = new Endpoint(endpointAddress);
        var transport = new RequestReplyTransport(
            _options.Serializer,
            serviceDescription,
            serviceEndpoint,
            endpoint);
        return new DuplexServiceChannel<TService, TServiceCallback>(
            this,
            _options.ConnectionFactory,
            transport,
            transport,
            endpointAddress);
    }

    public IServiceChannel<TService> CreateServiceChannel<TService>(
        ServiceDescription serviceDescription,
        ServiceEndpoint serviceEndpoint,
        EndpointAddress endpointAddress)
        where TService : class
    {
        var endpoint = new Endpoint(endpointAddress);
        var transport = new RequestReplyTransport(
            _options.Serializer,
            serviceDescription,
            serviceEndpoint,
            endpoint);
        return new ServiceChannel<TService>(
            _options.ConnectionFactory,
            transport,
            transport,
            endpointAddress);
    }
}