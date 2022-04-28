using System;

namespace SweepingBlade.Communication.Primitives.Annotations;

[AttributeUsage(ServiceModelAttributeTargets.ServiceContract, Inherited = false)]
public sealed class ServiceAttribute : Attribute
{
}