using System;
using System.Reflection;
using SweepingBlade.Communication.Primitives.Description;

namespace SweepingBlade.Communication.Primitives;

public interface IDescriptionResolver
{
    ServiceEndpoint CreateServiceEndpoint(Type type, EndpointAddress endpointAddress);
    OperationDescription ResolveOperation(MethodInfo methodInfo);
    ServiceDescription ResolveService(Type type);
}