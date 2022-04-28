using System;
using System.Reflection;
using SweepingBlade.Communication.Primitives.Serialization;

namespace SweepingBlade.Communication.Primitives.Behaviors;

public class SerializationContextOperationBehavior : IServiceBehavior
{
    private readonly ISerializer _serializer;

    public SerializationContextOperationBehavior(ISerializer serializer)
    {
        _serializer = serializer ?? throw new ArgumentNullException(nameof(serializer));
    }

    public object Invoke(object obj, MethodInfo targetMethod, object[] args, OperationInvocationDelegate next)
    {
        var operationContext = new SerializationContext(_serializer);
        SerializationContext.SetCurrent(operationContext);

        try
        {
            return next();
        }
        finally
        {
            SerializationContext.SetCurrent(null);
        }
    }
}