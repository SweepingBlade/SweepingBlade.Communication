using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using SweepingBlade.Communication.Primitives.Behaviors;
using SweepingBlade.Communication.Primitives.Collections.Generic;

namespace SweepingBlade.Communication.Primitives.Description;

public sealed class ServiceEndpoint
{
    public EndpointAddress Address { get; }
    public KeyedCollection<Type, IEndpointBehavior> Behaviors { get; }
    public object Instance { get; }
    public IReadOnlyCollection<OperationDescription> Operations { get; }

    public ServiceEndpoint(EndpointAddress address, object instance, IReadOnlyCollection<OperationDescription> operations)
        : this(address, operations)
    {
        Instance = instance ?? throw new ArgumentNullException(nameof(instance));
    }

    public ServiceEndpoint(EndpointAddress address, IReadOnlyCollection<OperationDescription> operations)
    {
        Address = address ?? throw new ArgumentNullException(nameof(address));
        Operations = operations ?? throw new ArgumentNullException(nameof(operations));

        Behaviors = new KeyedByTypeCollection<IEndpointBehavior>();
    }
}