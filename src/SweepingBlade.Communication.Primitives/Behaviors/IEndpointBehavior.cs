using System.Reflection;

namespace SweepingBlade.Communication.Primitives.Behaviors;

public interface IEndpointBehavior
{
    object Invoke(object obj, MethodInfo targetMethod, object[] args, OperationInvocationDelegate next);
}