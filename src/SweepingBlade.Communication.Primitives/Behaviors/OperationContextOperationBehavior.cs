using System;
using System.Reflection;
using SweepingBlade.Communication.Primitives.Channels;
using SweepingBlade.Communication.Primitives.Description;

namespace SweepingBlade.Communication.Primitives.Behaviors;

public class OperationContextOperationBehavior<TService> : IOperationBehavior
    where TService : class
{
    private readonly ICommunicationContext _channel;

    public OperationContextOperationBehavior(ICommunicationContext channel)
    {
        _channel = channel ?? throw new ArgumentNullException(nameof(channel));
    }

    void IOperationBehavior.AfterInvoke(object obj, MethodInfo targetMethod, object[] args, object result, AfterOperationInvocationDelegate next)
    {
        try
        {
            next();
        }
        finally
        {
            OperationContext<TService>.SetCurrent(null);
        }
    }

    void IOperationBehavior.BeforeInvoke(object obj, MethodInfo targetMethod, object[] args, EndpointAddress endpointAddress, BeforeOperationInvocationDelegate next)
    {
        var operationContext = new OperationContext<TService>(_channel);
        OperationContext<TService>.SetCurrent(operationContext);
        next();
    }
}

public class OperationContextOperationBehavior<TService, TServiceCallback> : IOperationBehavior
    where TService : class
    where TServiceCallback : class
{
    private readonly IDuplexServiceChannel<TService, TServiceCallback> _channel;
    private readonly ServiceDescription _serviceDescription;
    private readonly ServiceEndpoint _serviceEndpoint;

    public OperationContextOperationBehavior(IDuplexServiceChannel<TService, TServiceCallback> channel, ServiceDescription serviceDescription, ServiceEndpoint serviceEndpoint)
    {
        _channel = channel ?? throw new ArgumentNullException(nameof(channel));
        _serviceDescription = serviceDescription ?? throw new ArgumentNullException(nameof(serviceDescription));
        _serviceEndpoint = serviceEndpoint ?? throw new ArgumentNullException(nameof(serviceEndpoint));
    }

    void IOperationBehavior.AfterInvoke(object obj, MethodInfo targetMethod, object[] args, object result, AfterOperationInvocationDelegate next)
    {
        try
        {
            next();
        }
        finally
        {
            OperationContext<TService, TServiceCallback>.SetCurrent(null);
        }
    }

    void IOperationBehavior.BeforeInvoke(object obj, MethodInfo targetMethod, object[] args, EndpointAddress endpointAddress, BeforeOperationInvocationDelegate next)
    {
        var operationContext = new OperationContext<TService, TServiceCallback>(_channel, _serviceDescription, _serviceEndpoint, endpointAddress);
        OperationContext<TService, TServiceCallback>.SetCurrent(operationContext);
        next();
    }
}