using System;
using System.ComponentModel;
using SweepingBlade.Communication.Primitives.Serialization;

namespace SweepingBlade.Communication.Primitives.Behaviors;

public sealed class SerializationContext : ISerializer
{
    [ThreadStatic]
    private static ContextInstance _current;

    private readonly ISerializer _serializer;

    public static SerializationContext Current => _current.Value;

    public SerializationContext(ISerializer serializer)
    {
        _serializer = serializer ?? throw new ArgumentNullException(nameof(serializer));
    }

    public T Deserialize<T>(ReadOnlyMemory<byte> value)
    {
        return _serializer.Deserialize<T>(value);
    }

    public ReadOnlyMemory<byte> Serialize<T>(T value)
    {
        return _serializer.Serialize(value);
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public static void SetCurrent(SerializationContext value)
    {
        _current = new ContextInstance(value);
    }

    private sealed class ContextInstance
    {
        public SerializationContext Value { get; }

        public ContextInstance(SerializationContext value)
        {
            Value = value;
        }
    }
}