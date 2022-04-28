using System;

namespace SweepingBlade.Communication.Primitives;

public class FaultException : Exception
{
    public static readonly FaultException Default = new FaultException("The server was unable to process the request due to an internal error.");

    public FaultException(string message)
        : base(message)
    {
    }
}

public class FaultException<TDetail> : FaultException
{
    public TDetail Detail { get; }

    public FaultException(TDetail detail)
        : this(null, detail)
    {
    }

    public FaultException(string message, TDetail detail)
        : base(message)
    {
        Detail = detail ?? throw new ArgumentNullException(nameof(detail));
    }
}