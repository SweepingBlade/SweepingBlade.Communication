using System;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;

namespace SweepingBlade.Communication.Primitives;

public static class TaskExtensions
{
    public static async Task<T> ExecuteAsync<T>(this Task<T> task, TimeSpan timeout, CancellationToken cancellationToken, [CallerMemberName] string callerMemberName = null)
    {
        var timeoutTask = Task.Delay(timeout, cancellationToken);
        var finishedTask = await Task.WhenAny(timeoutTask, task);

        cancellationToken.ThrowIfCancellationRequested();

        if (finishedTask == timeoutTask)
        {
            throw new TimeoutException($"The operation '{callerMemberName}' timed out.");
        }

        return await task;
    }

    public static async Task ExecuteAsync(this Task task, TimeSpan timeout, CancellationToken cancellationToken, [CallerMemberName] string callerMemberName = null)
    {
        var timeoutTask = Task.Delay(timeout, cancellationToken);
        var finishedTask = await Task.WhenAny(timeoutTask, task);

        cancellationToken.ThrowIfCancellationRequested();

        if (finishedTask == timeoutTask)
        {
            throw new TimeoutException($"The operation '{callerMemberName}' timed out.");
        }

        await task;
    }
}