using System.Reflection;
using System.Threading.Tasks;
using SweepingBlade.Communication.Primitives.Channels;

namespace SweepingBlade.Communication.Primitives.Dispatcher;

public interface IOperationInvoker
{
    object Invoke(IConnection connection, MethodInfo targetMethod, object[] args);
    Task InvokeAsync(IConnection connection, MethodInfo targetMethod, object[] args);
    Task<T> InvokeAsync<T>(IConnection connection, MethodInfo targetMethod, object[] args);
}