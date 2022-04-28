using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using SweepingBlade.Communication.Primitives.Annotations;
using SweepingBlade.Communication.Primitives.Behaviors;
using SweepingBlade.Communication.Primitives.Description;

namespace SweepingBlade.Communication.Primitives;

public class DescriptionResolver : IDescriptionResolver
{
    public ServiceEndpoint CreateServiceEndpoint(Type type, EndpointAddress endpointAddress)
    {
        if (type is null) throw new ArgumentNullException(nameof(type));

        if (type.AssemblyQualifiedName is null)
        {
            throw new ArgumentException($"The type does not have '{nameof(Type.AssemblyQualifiedName)}'.", nameof(type));
        }

        var operationDescriptions = new List<OperationDescription>();
        foreach (var methodInfo in type.GetMethods(BindingFlags.Public | BindingFlags.Instance))
        {
            operationDescriptions.Add(ResolveOperation(methodInfo));
        }

        var serviceEndpoint = new ServiceEndpoint(endpointAddress, type, operationDescriptions);

        // ReSharper disable once SuspiciousTypeConversion.Global
        foreach (var endpointBehavior in type.GetCustomAttributes().OfType<IEndpointBehavior>())
        {
            serviceEndpoint.Behaviors.Add(endpointBehavior);
        }

        return serviceEndpoint;
    }

    public OperationDescription ResolveOperation(MethodInfo methodInfo)
    {
        var operationAttribute = methodInfo.GetCustomAttribute<OperationAttribute>();

        if (operationAttribute is not null)
        {
            if (operationAttribute.IsOneWay && methodInfo.ReturnType != PrimitiveTypeManager.VoidType)
            {
                throw new InvalidOperationException($"The one-way operation '{methodInfo.Name}' must have a 'void' return type.");
            }
        }

        var operationDescription = new OperationDescription(operationAttribute?.Name ?? methodInfo.Name, methodInfo, operationAttribute?.IsOneWay == true);

        // ReSharper disable once SuspiciousTypeConversion.Global
        foreach (var operationBehavior in methodInfo.GetCustomAttributes().OfType<IOperationBehavior>())
        {
            operationDescription.Behaviors.Add(operationBehavior);
        }

        return operationDescription;
    }

    public ServiceDescription ResolveService(Type type)
    {
        var serviceDescription = new ServiceDescription();

        foreach (var serviceBehavior in type.GetCustomAttributes().OfType<IServiceBehavior>())
        {
            serviceDescription.Behaviors.Add(serviceBehavior);
        }

        return serviceDescription;
    }
}