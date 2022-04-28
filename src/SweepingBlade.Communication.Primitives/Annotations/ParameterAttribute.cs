using System;

namespace SweepingBlade.Communication.Primitives.Annotations;

[AttributeUsage(ServiceModelAttributeTargets.Parameter)]
public sealed class ParameterAttribute : Attribute
{
}