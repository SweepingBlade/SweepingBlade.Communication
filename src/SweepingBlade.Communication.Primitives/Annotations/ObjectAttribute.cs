using System;

namespace SweepingBlade.Communication.Primitives.Annotations;

[AttributeUsage(ServiceModelAttributeTargets.DataContract, Inherited = false)]
public sealed class ObjectAttribute : Attribute
{
}