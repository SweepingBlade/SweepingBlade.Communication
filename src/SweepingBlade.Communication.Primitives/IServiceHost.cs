using System;
using SweepingBlade.Communication.Primitives.Channels;

namespace SweepingBlade.Communication.Primitives;

public interface IServiceHost : ICommunication
{
    void AddServiceEndpoint<TService>(TService instance, Options options, Uri uri)
        where TService : class;

    void AddServiceEndpoint<TService, TServiceCallback>(TService instance, Options options, Uri uri)
        where TService : class
        where TServiceCallback : class;
}