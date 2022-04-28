namespace SweepingBlade.Communication.Primitives.Messaging;

public sealed class OperationInvocationResponse
{
    public ErrorValue Error { get; }
    public PrimitiveValue Return { get; }

    public OperationInvocationResponse(PrimitiveValue @return, ErrorValue error)
    {
        Return = @return;
        Error = error;
    }
}