using System;
using System.Collections.Generic;

namespace SweepingBlade.Communication.Primitives.Messaging;

public sealed class OperationInvocationRequest
{
    public string MethodName { get; }
    public IReadOnlyList<Parameter> Parameters { get; }

    public OperationInvocationRequest(string methodName, IReadOnlyList<Parameter> parameters)
    {
        MethodName = methodName ?? throw new ArgumentNullException(nameof(methodName));
        Parameters = parameters ?? throw new ArgumentNullException(nameof(parameters));
    }

    public sealed class Parameter
    {
        public string Name { get; }
        public PrimitiveValue PrimitiveValue { get; }

        public Parameter(string name, PrimitiveValue primitiveValue)
        {
            Name = name ?? throw new ArgumentNullException(nameof(name));
            PrimitiveValue = primitiveValue ?? throw new ArgumentNullException(nameof(primitiveValue));
        }
    }
}