using System;

namespace SweepingBlade.Communication.Primitives;

public class ExceptionDetails
{
    public Exception InnerException { get; }
    public string Message { get; }
    public string StackTrace { get; }

    public ExceptionDetails(Exception innerException, string message, string stackTrace)
    {
        Message = message ?? throw new ArgumentNullException(nameof(message));
        StackTrace = stackTrace ?? throw new ArgumentNullException(nameof(stackTrace));
        InnerException = innerException ?? throw new ArgumentNullException(nameof(innerException));
    }

    public ExceptionDetails(Exception innerException)
    {
        InnerException = innerException ?? throw new ArgumentNullException(nameof(innerException));
        Message = innerException.Message;
        StackTrace = innerException.StackTrace;
    }
}