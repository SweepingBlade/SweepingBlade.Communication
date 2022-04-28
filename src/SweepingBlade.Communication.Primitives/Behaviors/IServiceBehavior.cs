using System.Reflection;

namespace SweepingBlade.Communication.Primitives.Behaviors;

public interface IServiceBehavior
{
    object Invoke(object obj, MethodInfo targetMethod, object[] args, OperationInvocationDelegate next);
}