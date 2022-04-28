using System;
using System.Collections.ObjectModel;
using System.Reflection;
using SweepingBlade.Communication.Primitives.Behaviors;
using SweepingBlade.Communication.Primitives.Collections.Generic;

namespace SweepingBlade.Communication.Primitives.Description;

public class OperationDescription
{
    public KeyedCollection<Type, IOperationBehavior> Behaviors { get; }
    public bool IsOneWay { get; }
    public MethodInfo MethodInfo { get; }
    public string Name { get; }

    public OperationDescription(string name, MethodInfo methodInfo, bool isOneWay)
    {
        Name = name ?? throw new ArgumentNullException(nameof(name));
        MethodInfo = methodInfo ?? throw new ArgumentNullException(nameof(methodInfo));
        IsOneWay = isOneWay;

        Behaviors = new KeyedByTypeCollection<IOperationBehavior>();
    }
}