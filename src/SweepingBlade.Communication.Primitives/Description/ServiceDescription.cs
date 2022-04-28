using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using SweepingBlade.Communication.Primitives.Behaviors;
using SweepingBlade.Communication.Primitives.Collections.Generic;

namespace SweepingBlade.Communication.Primitives.Description;

public class ServiceDescription
{
    private readonly ConcurrentBag<ServiceEndpoint> _endpoints;

    public KeyedCollection<Type, IServiceBehavior> Behaviors { get; }
    public IEnumerable<ServiceEndpoint> Endpoints => _endpoints;

    public ServiceDescription()
    {
        Behaviors = new KeyedByTypeCollection<IServiceBehavior>();
        _endpoints = new ConcurrentBag<ServiceEndpoint>();
    }

    public void AddEndpoint(ServiceEndpoint endpoint)
    {
        if (endpoint is null) throw new ArgumentNullException(nameof(endpoint));
        _endpoints.Add(endpoint);
    }
}