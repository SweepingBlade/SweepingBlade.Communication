using System;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using SweepingBlade.Communication.Primitives.Channels;
using SweepingBlade.Communication.Primitives.Description;
using SweepingBlade.Communication.Primitives.Dispatcher;
using SweepingBlade.Communication.Primitives.Messaging;

namespace SweepingBlade.Communication.Primitives.Transports;

public partial class RequestReplyTransport : IOperationInvoker
{
    public object Invoke(IConnection connection, MethodInfo targetMethod, object[] args)
    {
        var operationDescription = ResolveOperationDescription(targetMethod);
        var taskCompletionSource = Invoke(connection, operationDescription, targetMethod, args);
        return !operationDescription.IsOneWay ? taskCompletionSource.Task.GetAwaiter().GetResult() : null;
    }

    public async Task InvokeAsync(IConnection connection, MethodInfo targetMethod, object[] args)
    {
        var operationDescription = ResolveOperationDescription(targetMethod);
        var taskCompletionSource = Invoke(connection, operationDescription, targetMethod, args);
        await taskCompletionSource.Task;
    }

    public async Task<T> InvokeAsync<T>(IConnection connection, MethodInfo targetMethod, object[] args)
    {
        var operationDescription = ResolveOperationDescription(targetMethod);
        var taskCompletionSource = Invoke(connection, operationDescription, targetMethod, args);
        return (T)await taskCompletionSource.Task;
    }

    private OperationInvocationRequestMessage CreateOperationInvocationRequestMessage(string methodName, System.Collections.Generic.IEnumerable<ParameterInfo> parameterInfos, System.Collections.Generic.IReadOnlyList<object> args)
    {
        var parameters = parameterInfos.Select((parameter, index) =>
        {
            var value = args[index];
            var valueType = PrimitiveTypeManager.GetTypeOrDefault(value);
            var parameterValue = CreatePrimitiveValue(value, valueType);
            return new OperationInvocationRequest.Parameter(parameter.Name, parameterValue);
        }).ToList();
        var request = new OperationInvocationRequest(methodName, parameters);
        return new OperationInvocationRequestMessage(_endpoint, request);
    }

    private TaskCompletionSource<object> Invoke(IConnection connection, OperationDescription operationDescription, MethodInfo targetMethod, object[] args)
    {
        var parameterInfos = targetMethod.GetParameters();

        if (args.Length != parameterInfos.Length)
        {
            throw new NotSupportedException($"Parameters modifiers 'out' and 'ref' are not supported by {nameof(RequestReplyTransport)}");
        }

        var requestMessage = CreateOperationInvocationRequestMessage(operationDescription.Name, parameterInfos, args);
        var endpointMessage = CreateEndpointMessage(Guid.NewGuid(), requestMessage);
        var taskCompletionSource = new TaskCompletionSource<object>();

        if (!operationDescription.IsOneWay)
        {
            if (!_messageById.TryAdd(endpointMessage.Id, endpointMessage))
            {
                throw new InvalidOperationException("Could not invoke operation.");
            }

            if (!_taskCompletionSourceByMessageId.TryAdd(endpointMessage.Id, taskCompletionSource))
            {
                throw new InvalidOperationException("Could not invoke operation.");
            }
        }

        var data = Serialize(endpointMessage);
        _ = connection.SendAsync(data, CancellationToken.None);

        return taskCompletionSource;
    }

    private OperationDescription ResolveOperationDescription(MethodInfo targetMethod)
    {
        return _serviceEndpoint.Operations.First(operationDescription => operationDescription.Name == targetMethod.Name);
    }
}