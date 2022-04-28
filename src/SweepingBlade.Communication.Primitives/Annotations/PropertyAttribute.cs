using System;

namespace SweepingBlade.Communication.Primitives.Annotations;

[AttributeUsage(ServiceModelAttributeTargets.DataMember)]
public sealed class PropertyAttribute : Attribute
{
}