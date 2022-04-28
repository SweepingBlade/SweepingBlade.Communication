using System;
using System.Reflection;

namespace SweepingBlade.Communication.Primitives.Behaviors;

[AttributeUsage(ServiceModelAttributeTargets.ServiceBehavior, Inherited = false)]
public sealed class ServiceBehaviorAttribute : Attribute, IServiceBehavior
{
    public bool IncludeExceptionDetailsInFaults { get; set; }

    object IServiceBehavior.Invoke(object obj, MethodInfo targetMethod, object[] args, OperationInvocationDelegate next)
    {
        if (!IncludeExceptionDetailsInFaults)
        {
            return next();
        }

        try
        {
            return next();
        }
        catch (TargetInvocationException ex) when (ex.InnerException is FaultException)
        {
            // Ignore (explicit) fault exceptions
            throw;
        }
        catch (TargetInvocationException ex) when (ex.InnerException is not null)
        {
            var exceptionDetail = new ExceptionDetails(ex.InnerException);
            throw new FaultException<ExceptionDetails>(exceptionDetail);
        }
    }
}