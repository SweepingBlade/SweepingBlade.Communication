using System;
using System.Threading.Tasks;

namespace SweepingBlade.Communication.Primitives;

public class PrimitiveTypeManager
{
    public static readonly Type ObjectType = typeof(object);
    public static readonly Type TaskType = typeof(Task);
    public static readonly Type VoidType = typeof(void);

    public static Type GetTypeOrDefault(object value)
    {
        return value?.GetType() ?? ObjectType;
    }
}