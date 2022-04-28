using System;

namespace SweepingBlade.Communication.Primitives.Annotations;

[AttributeUsage(ServiceModelAttributeTargets.OperationContract)]
public sealed class OperationAttribute : Attribute
{
    public bool IsOneWay { get; set; }
    public string Name { get; set; }
}