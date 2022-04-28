using SweepingBlade.Communication.Primitives.Dispatcher;

namespace SweepingBlade.Communication.Primitives.Channels;

public interface IConnectionFactory
{
    IConnection CreateClientConnection(IOperationInvoker operationInvoker, IOperationHandler operationHandler, EndpointAddress endpointAddress);
    IConnection CreateServerConnection(IOperationInvoker operationInvoker, IOperationHandler operationHandler, EndpointAddress endpointAddress);
}