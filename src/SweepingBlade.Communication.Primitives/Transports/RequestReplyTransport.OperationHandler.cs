using System;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using SweepingBlade.Communication.Primitives.Behaviors;
using SweepingBlade.Communication.Primitives.Channels;
using SweepingBlade.Communication.Primitives.Description;
using SweepingBlade.Communication.Primitives.Dispatcher;
using SweepingBlade.Communication.Primitives.Messaging;

namespace SweepingBlade.Communication.Primitives.Transports;

public partial class RequestReplyTransport : IOperationHandler
{
    public async Task HandleAsync(IConnection connection, ReadOnlyMemory<byte> data, CancellationToken cancellationToken)
    {
        var message = Deserialize(data);

        switch (message!.Message)
        {
            case OperationInvocationRequestMessage requestMessage:
                await HandleMessageAsync(connection, message, requestMessage, cancellationToken);
                break;
            case OperationInvocationResponseMessage responseMessage:
                await HandleMessageAsync(message, responseMessage, cancellationToken);
                break;
            default:
                throw new InvalidOperationException("Invalid message.");
        }
    }

    private static EndpointMessage CreateEndpointMessage(Guid id, Message message)
    {
        return new EndpointMessage(id, message);
    }

    private static ErrorValue CreateErrorValue(FaultException faultException)
    {
        return faultException is not null ? new ErrorValue(faultException) : null;
    }

    private static object HandleInvocation(object obj, MethodInfo methodInfo, object[] args)
    {
        return methodInfo.Invoke(obj, args);
    }

    private static async Task HandleInvocationAsync(Task task)
    {
        await task.ConfigureAwait(false);
    }

    private static async Task<T> HandleInvocationAsync<T>(Task<T> task)
    {
        return await task.ConfigureAwait(false);
    }

    private OperationInvocationResponseMessage CreateOperationInvocationResponseMessage(PrimitiveValue @return, FaultException faultException)
    {
        var errorValue = CreateErrorValue(faultException);
        var operationInvocationResponse = new OperationInvocationResponse(@return, errorValue);
        return new OperationInvocationResponseMessage(_endpoint, operationInvocationResponse);
    }

    private object HandleInvocation(OperationDescription operationDescription, object[] args, EndpointAddress endpointAddress)
    {
        HandleBeforeInvokeOperationBehaviors();

        object result = null;

        try
        {
            result = HandleInvocationOperationBehaviors();
        }
        finally
        {
            HandlePostInvocationOperationBehaviors();
        }

        return result;

        void HandleBeforeInvokeOperationBehaviors()
        {
            var currentDelegate = new BeforeOperationInvocationDelegate(() => { });

            foreach (var operationBehavior in operationDescription.Behaviors)
            {
                var @delegate = currentDelegate;
                currentDelegate = () => operationBehavior.BeforeInvoke(_serviceEndpoint.Instance, operationDescription.MethodInfo, args, endpointAddress, @delegate);
            }

            currentDelegate();
        }

        object HandleInvocationOperationBehaviors()
        {
            var currentDelegate = new OperationInvocationDelegate(() => HandleInvocation(_serviceEndpoint.Instance, operationDescription.MethodInfo, args));

            foreach (var operationBehavior in _serviceDescription.Behaviors)
            {
                var @delegate = currentDelegate;
                currentDelegate = () => operationBehavior.Invoke(_serviceEndpoint.Instance, operationDescription.MethodInfo, args, @delegate);
            }

            foreach (var endpointBehavior in _serviceEndpoint.Behaviors)
            {
                var @delegate = currentDelegate;
                currentDelegate = () => endpointBehavior.Invoke(_serviceEndpoint.Instance, operationDescription.MethodInfo, args, @delegate);
            }

            return currentDelegate();
        }

        void HandlePostInvocationOperationBehaviors()
        {
            var currentDelegate = new AfterOperationInvocationDelegate(() => { });

            foreach (var operationBehavior in operationDescription.Behaviors)
            {
                var @delegate = currentDelegate;
                currentDelegate = () => operationBehavior.AfterInvoke(_serviceEndpoint.Instance, operationDescription.MethodInfo, args, result, @delegate);
            }

            currentDelegate();
        }
    }

    private Task HandleMessageAsync(EndpointMessage message, OperationInvocationResponseMessage responseMessage, CancellationToken cancellationToken)
    {
        if (!_messageById.TryRemove(message.Id, out var originalMessage))
        {
            throw new InvalidOperationException("Could not process response.");
        }

        if (originalMessage.Message is not OperationInvocationRequestMessage requestMessage)
        {
            throw new InvalidOperationException("Could not process response.");
        }

        if (!_taskCompletionSourceByMessageId.TryRemove(message.Id, out var taskCompletionSource))
        {
            throw new InvalidOperationException("Could not process response.");
        }

        if (responseMessage.Response.Error is not null)
        {
            taskCompletionSource.SetException(responseMessage.Response.Error.Exception);
            return null;
        }

        var operationDescription = _serviceEndpoint.Operations.First(operationDescription => operationDescription.Name == requestMessage.Request.MethodName);

        object result;
        if (operationDescription.MethodInfo.ReturnType == PrimitiveTypeManager.TaskType || operationDescription.MethodInfo.ReturnType == PrimitiveTypeManager.VoidType)
        {
            result = null;
        }
        else
        {
            result = responseMessage.Response.Return?.Value;
        }

        taskCompletionSource.SetResult(result);
        return Task.CompletedTask;
    }

    private async Task HandleMessageAsync(IConnection connection, EndpointMessage message, OperationInvocationRequestMessage requestMessage, CancellationToken cancellationToken)
    {
        var operationDescription = _serviceEndpoint.Operations.First(operationDescription => operationDescription.Name == requestMessage.Request.MethodName);

        object result = null;
        FaultException faultException = null;
        var args = requestMessage.Request.Parameters.Select(param => param.PrimitiveValue?.Value).ToArray();

        try
        {
            var invocation = HandleInvocation(operationDescription, args, requestMessage.Endpoint.CallbackAddress);

            if (operationDescription.IsOneWay)
            {
                return;
            }

            if (operationDescription.MethodInfo.ReturnType.IsSubclassOf(PrimitiveTypeManager.TaskType))
            {
                var methodInfo = HandleInvocationAsyncMethodInfo.MakeGenericMethod(operationDescription.MethodInfo.ReturnType.GetGenericArguments()[0]);
                var taskResult = methodInfo.Invoke(null, new[] { invocation });
                var propertyInfo = taskResult!.GetType().GetProperty(nameof(Task<object>.Result));

                result = propertyInfo!.GetValue(invocation);
            }
            else if (operationDescription.MethodInfo.ReturnType == PrimitiveTypeManager.TaskType)
            {
                await HandleInvocationAsync((Task)invocation);
            }
            else
            {
                result = invocation;
            }
        }
        catch (TargetInvocationException ex) when (ex.InnerException is FaultException innerException)
        {
            // Explicit exception thrown by the invoker synchronously
            faultException = innerException;
        }
        catch (TargetInvocationException ex) when (ex.InnerException is AggregateException { InnerException: FaultException innerException })
        {
            // Explicit exception thrown by the invoker asynchronously
            faultException = innerException;
        }
        catch (FaultException ex)
        {
            faultException = ex;
        }
        catch (Exception)
        {
            faultException = FaultException.Default;
        }

        var primitiveValue = CreatePrimitiveValue(result, operationDescription.MethodInfo.ReturnType);
        var responseMessage = CreateOperationInvocationResponseMessage(primitiveValue, faultException);
        var endpointMessage = CreateEndpointMessage(message.Id, responseMessage);
        var data = Serialize(endpointMessage);
        await connection.SendAsync(data, cancellationToken);
    }
}