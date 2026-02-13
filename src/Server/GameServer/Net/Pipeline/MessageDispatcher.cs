using EclipseOriginsModern.Server.GameServer.Net.Sessions;
using EclipseOriginsModern.Shared.Abstractions.Net;

namespace EclipseOriginsModern.Server.GameServer.Net.Pipeline;

public sealed class MessageDispatcher
{
    private readonly Dictionary<ushort, Func<Session, ReadOnlyMemory<byte>, CancellationToken, Task>> _handlers = new();

    public void RegisterHandler(ushort messageType, Func<Session, ReadOnlyMemory<byte>, CancellationToken, Task> handler)
    {
        _handlers[messageType] = handler;
    }

    public async Task DispatchAsync(Session session, Frame frame, CancellationToken cancellationToken)
    {
        if (_handlers.TryGetValue(frame.MessageType, out var handler))
        {
            await handler(session, frame.Payload, cancellationToken);
            return;
        }

        // Unknown message type: ignore in this baseline, can be upgraded to explicit protocol error.
        await Task.CompletedTask;
    }
}
