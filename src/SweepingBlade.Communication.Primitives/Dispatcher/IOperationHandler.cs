using System;
using System.Threading;
using System.Threading.Tasks;
using SweepingBlade.Communication.Primitives.Channels;

namespace SweepingBlade.Communication.Primitives.Dispatcher;

public interface IOperationHandler
{
    Task HandleAsync(IConnection connection, ReadOnlyMemory<byte> data, CancellationToken cancellationToken);
}