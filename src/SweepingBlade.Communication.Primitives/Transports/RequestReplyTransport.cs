using System;
using System.Collections.Concurrent;
using System.Reflection;
using System.Threading.Tasks;
using SweepingBlade.Communication.Primitives.Channels;
using SweepingBlade.Communication.Primitives.Description;
using SweepingBlade.Communication.Primitives.Messaging;
using SweepingBlade.Communication.Primitives.Serialization;

namespace SweepingBlade.Communication.Primitives.Transports;

public partial class RequestReplyTransport : CommunicationContext
{
    private static readonly MethodInfo DeserializeEndpointMessageMethodInfo;
    private static readonly MethodInfo HandleInvocationAsyncMethodInfo;
    private static readonly MethodInfo SerializeEndpointMessageMethodInfo;
    private readonly Endpoint _endpoint;
    private readonly ConcurrentDictionary<Guid, EndpointMessage> _messageById;
    private readonly ISerializer _serializer;
    private readonly ServiceDescription _serviceDescription;
    private readonly ServiceEndpoint _serviceEndpoint;
    private readonly ConcurrentDictionary<Guid, TaskCompletionSource<object>> _taskCompletionSourceByMessageId;

    public RequestReplyTransport(
        ISerializer serializer,
        ServiceDescription serviceDescription,
        ServiceEndpoint serviceEndpoint,
        Endpoint endpoint)
    {
        _serializer = serializer ?? throw new ArgumentNullException(nameof(serializer));
        _serviceDescription = serviceDescription ?? throw new ArgumentNullException(nameof(serviceDescription));
        _serviceEndpoint = serviceEndpoint ?? throw new ArgumentNullException(nameof(serviceEndpoint));
        _endpoint = endpoint ?? throw new ArgumentNullException(nameof(endpoint));

        _messageById = new ConcurrentDictionary<Guid, EndpointMessage>();
        _taskCompletionSourceByMessageId = new ConcurrentDictionary<Guid, TaskCompletionSource<object>>();
    }

    static RequestReplyTransport()
    {
        var serializerType = typeof(ISerializer);

        var serializeMethodInfo = serializerType.GetMethod(nameof(ISerializer.Serialize));
        SerializeEndpointMessageMethodInfo = serializeMethodInfo!.MakeGenericMethod(TypeManager.EndpointMessageType);

        var deserializeMethodInfo = serializerType.GetMethod(nameof(ISerializer.Deserialize));
        DeserializeEndpointMessageMethodInfo = deserializeMethodInfo!.MakeGenericMethod(TypeManager.EndpointMessageType);

        HandleInvocationAsyncMethodInfo = typeof(RequestReplyTransport).GetMethod(nameof(HandleInvocationAsync));
    }

    private static PrimitiveValue CreatePrimitiveValue(object value, Type type)
    {
        if (value is null || type == PrimitiveTypeManager.VoidType || type == PrimitiveTypeManager.TaskType)
        {
            return null;
        }

        return new PrimitiveValue(value);
    }

    private EndpointMessage Deserialize(ReadOnlyMemory<byte> data)
    {
        return (EndpointMessage)DeserializeEndpointMessageMethodInfo.Invoke(_serializer, new object[] { data });
    }

    private ReadOnlyMemory<byte> Serialize(EndpointMessage message)
    {
        var data = (ReadOnlyMemory<byte>)SerializeEndpointMessageMethodInfo.Invoke(_serializer, new object[] { message })!;
        return data;
    }
}