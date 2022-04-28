namespace SweepingBlade.Communication.Primitives.Messaging;

public sealed class PrimitiveValue
{
    public object Value { get; }

    public PrimitiveValue(object value)
    {
        Value = value;
    }
}