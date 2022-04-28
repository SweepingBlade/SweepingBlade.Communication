using System;
using System.Reflection;
using System.Threading.Tasks;
using SweepingBlade.Communication.Primitives.Channels;

namespace SweepingBlade.Communication.Primitives.Dispatcher;

public abstract class DispatchProxy : System.Reflection.DispatchProxy
{
    protected abstract object Invoke(object obj, MethodInfo targetMethod, object[] args);
    protected abstract Task InvokeAsync(object obj, MethodInfo targetMethod, object[] args);
    protected abstract Task<T> InvokeAsync<T>(object obj, MethodInfo targetMethod, object[] args);

    private static readonly Type TaskType;

    private MethodInfo InvokeAsyncMethodInfo => GetType().GetMethod(nameof(InvokeAsync));

    static DispatchProxy()
    {
        TaskType = typeof(Task);
    }

    protected sealed override object Invoke(MethodInfo targetMethod, object[] args)
    {
        if (targetMethod.ReturnType.IsSubclassOf(TaskType))
        {
            var genericMethodInfo = InvokeAsyncMethodInfo.MakeGenericMethod(targetMethod.ReturnType.GetGenericArguments()[0]);
            var genericMethodArgs = new object[args.Length + 1];
            genericMethodArgs[0] = this;
            Array.Copy(args, 0, genericMethodArgs, 1, args.Length);
            return genericMethodInfo.Invoke(null, genericMethodArgs);
        }

        if (targetMethod.ReturnType == TaskType)
        {
            return InvokeAsync(this, targetMethod, args);
        }

        return Invoke(this, targetMethod, args);
    }
}

public class DispatchProxy<TService> : DispatchProxy
{
    private IConnection _connection;

    protected override object Invoke(object obj, MethodInfo targetMethod, object[] args)
    {
        return _connection?.Invoke(obj, targetMethod, args);
    }

    protected override Task InvokeAsync(object obj, MethodInfo targetMethod, object[] args)
    {
        return _connection.InvokeAsync(obj, targetMethod, args);
    }

    protected override Task<T> InvokeAsync<T>(object obj, MethodInfo targetMethod, object[] args)
    {
        return _connection.InvokeAsync<T>(obj, targetMethod, args);
    }

    public static TService Create(IConnection connection)
    {
        if (connection is null) throw new ArgumentNullException(nameof(connection));

        var proxy = Create<TService, DispatchProxy<TService>>();
        if (proxy is not DispatchProxy<TService> dispatchProxy) throw new InvalidOperationException(nameof(proxy));

        dispatchProxy._connection = connection;
        return proxy;
    }
}