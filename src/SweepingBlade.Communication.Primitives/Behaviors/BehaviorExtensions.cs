using SweepingBlade.Communication.Primitives.Channels;
using SweepingBlade.Communication.Primitives.Description;
using SweepingBlade.Communication.Primitives.Serialization;

namespace SweepingBlade.Communication.Primitives.Behaviors;

public static class BehaviorExtensions
{
    public static void ApplyDefaultBehaviors<TService>(
        this ICommunicationContext channel,
        ServiceDescription serviceDescription,
        ISerializer serializer)
        where TService : class
    {
        ApplySerializationBehavior(serviceDescription, serializer);

        foreach (var serviceEndpoint in serviceDescription.Endpoints)
        {
            foreach (var operationDescription in serviceEndpoint.Operations)
            {
                operationDescription.Behaviors.Add(new OperationContextOperationBehavior<TService>(channel));
            }
        }
    }

    public static void ApplyDefaultBehaviors<TService, TServiceCallback>(
        this IDuplexServiceChannel<TService, TServiceCallback> channel,
        ServiceDescription serviceDescription,
        ServiceDescription callbackServiceDescription,
        ServiceEndpoint callbackServiceEndpoint,
        ISerializer serializer)
        where TService : class
        where TServiceCallback : class
    {
        ApplySerializationBehavior(serviceDescription, serializer);

        foreach (var serviceEndpoint in serviceDescription.Endpoints)
        {
            foreach (var operationDescription in serviceEndpoint.Operations)
            {
                operationDescription.Behaviors.Add(new OperationContextOperationBehavior<TService, TServiceCallback>(channel, callbackServiceDescription, callbackServiceEndpoint));
            }
        }
    }

    private static void ApplySerializationBehavior(ServiceDescription serviceDescription, ISerializer serializer)
    {
        serviceDescription.Behaviors.Add(new SerializationContextOperationBehavior(serializer));
    }
}