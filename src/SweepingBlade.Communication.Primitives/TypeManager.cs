using System;
using SweepingBlade.Communication.Primitives.Messaging;

namespace SweepingBlade.Communication.Primitives;

public class TypeManager : PrimitiveTypeManager
{
    public static readonly Type EndpointMessageType = typeof(EndpointMessage);
}