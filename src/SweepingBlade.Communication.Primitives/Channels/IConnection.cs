using System;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

namespace SweepingBlade.Communication.Primitives.Channels;

public interface IConnection : ICommunicationContext
{
    Task HandleAsync(ReadOnlyMemory<byte> data, CancellationToken cancellationToken);
    object Invoke(object obj, MethodInfo targetMethod, object[] args);
    Task InvokeAsync(object obj, MethodInfo targetMethod, object[] args);
    Task<T> InvokeAsync<T>(object obj, MethodInfo targetMethod, object[] args);
    Task SendAsync(ReadOnlyMemory<byte> data, CancellationToken cancellationToken = default);
    void StartListening(CancellationToken? cancellationToken = default);
}