using System;

namespace SweepingBlade.Communication.Primitives.Serialization;

public interface ISerializer
{
    T Deserialize<T>(ReadOnlyMemory<byte> value);
    ReadOnlyMemory<byte> Serialize<T>(T value);
}