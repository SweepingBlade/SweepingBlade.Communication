using System.Reflection;

namespace SweepingBlade.Communication.Primitives.Behaviors;

public interface IOperationBehavior
{
    void AfterInvoke(object obj, MethodInfo targetMethod, object[] args, object result, AfterOperationInvocationDelegate next);
    void BeforeInvoke(object obj, MethodInfo targetMethod, object[] args, EndpointAddress endpointAddress, BeforeOperationInvocationDelegate next);
}