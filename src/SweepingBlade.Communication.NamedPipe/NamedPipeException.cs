using System;
using System.IO;
using System.Runtime.Serialization;

namespace SweepingBlade.Communication.NamedPipe;

public class NamedPipeException : IOException
{
    public NamedPipeException()
    {
    }

    public NamedPipeException(string message)
        : base(message)
    {
    }

    public NamedPipeException(string message, Exception innerException)
        : base(message, innerException)
    {
    }

    public NamedPipeException(string message, int hresult)
        : base(message, hresult)
    {
    }

    protected NamedPipeException(SerializationInfo info, StreamingContext context)
        : base(info, context)
    {
    }
}