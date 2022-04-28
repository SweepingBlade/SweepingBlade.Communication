using System;
using System.ComponentModel;
using SweepingBlade.Communication.Primitives.Channels;
using SweepingBlade.Communication.Primitives.Description;

namespace SweepingBlade.Communication.Primitives.Behaviors;

public sealed class OperationContext<TService>
    where TService : class
{
    [ThreadStatic]
    private static ContextInstance _current;

    public static OperationContext<TService> Current => _current.Value;
    public ICommunicationContext Channel { get; }

    public OperationContext(ICommunicationContext channel)
    {
        Channel = channel ?? throw new ArgumentNullException(nameof(channel));
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public static void SetCurrent(OperationContext<TService> value)
    {
        _current = new ContextInstance(value);
    }

    private sealed class ContextInstance
    {
        public OperationContext<TService> Value { get; }

        public ContextInstance(OperationContext<TService> value)
        {
            Value = value;
        }
    }
}

public sealed class OperationContext<TService, TServiceCallback>
    where TService : class
    where TServiceCallback : class
{
    [ThreadStatic]
    private static ContextInstance _current;

    private readonly IDuplexServiceChannel<TService, TServiceCallback> _channel;
    private readonly EndpointAddress _endpointAddress;
    private readonly ServiceDescription _serviceDescription;
    private readonly ServiceEndpoint _serviceEndpoint;

    public static OperationContext<TService, TServiceCallback> Current => _current.Value;
    public ICommunicationContext Channel => _channel;

    public OperationContext(IDuplexServiceChannel<TService, TServiceCallback> channel, ServiceDescription serviceDescription, ServiceEndpoint serviceEndpoint, EndpointAddress endpointAddress)
    {
        _channel = channel ?? throw new ArgumentNullException(nameof(channel));
        _serviceDescription = serviceDescription ?? throw new ArgumentNullException(nameof(serviceDescription));
        _serviceEndpoint = serviceEndpoint ?? throw new ArgumentNullException(nameof(serviceEndpoint));
        _endpointAddress = endpointAddress ?? throw new ArgumentNullException(nameof(endpointAddress));
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public static void SetCurrent(OperationContext<TService, TServiceCallback> value)
    {
        _current = new ContextInstance(value);
    }

    public IClientChannel<TServiceCallback> GetCallbackChannel()
    {
        return _channel.CreateClientChannel(_serviceDescription, _serviceEndpoint, _endpointAddress);
    }

    private sealed class ContextInstance
    {
        public OperationContext<TService, TServiceCallback> Value { get; }

        public ContextInstance(OperationContext<TService, TServiceCallback> value)
        {
            Value = value;
        }
    }
}