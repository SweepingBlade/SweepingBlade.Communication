using System;

namespace SweepingBlade.Communication.Primitives;

public static class ServiceModelAttributeTargets
{
    public const AttributeTargets DataContract = AttributeTargets.Class | AttributeTargets.Struct;
    public const AttributeTargets DataMember = AttributeTargets.Property | AttributeTargets.Field;
    public const AttributeTargets OperationContract = AttributeTargets.Method;
    public const AttributeTargets Parameter = AttributeTargets.ReturnValue | AttributeTargets.Parameter;
    public const AttributeTargets ServiceContract = AttributeTargets.Interface | AttributeTargets.Class;

    public const AttributeTargets ServiceBehavior = AttributeTargets.Class;
    public const AttributeTargets CallbackBehavior = AttributeTargets.Class;
    public const AttributeTargets ClientBehavior = AttributeTargets.Interface;
    public const AttributeTargets ContractBehavior = ServiceBehavior | ClientBehavior;
    public const AttributeTargets OperationBehavior = AttributeTargets.Method;
}