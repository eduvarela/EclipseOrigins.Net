using System.Collections.Concurrent;
using System.Net;
using System.Net.Sockets;
using EclipseOriginsModern.Server.GameServer.Net.Pipeline;
using EclipseOriginsModern.Server.GameServer.Net.Sessions;

namespace EclipseOriginsModern.Server.GameServer.Net.Tcp;

public sealed class TcpServer : IAsyncDisposable
{
    private readonly ConcurrentDictionary<string, Session> _sessions = new();
    private readonly MessageDispatcher _dispatcher;
    private readonly TcpListener _listener;

    public TcpServer(IPEndPoint endPoint, MessageDispatcher dispatcher)
    {
        _dispatcher = dispatcher;
        _listener = new TcpListener(endPoint);
    }

    public int SessionCount => _sessions.Count;

    public async Task RunAsync(CancellationToken cancellationToken)
    {
        _listener.Start();

        try
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                var client = await _listener.AcceptTcpClientAsync(cancellationToken);
                var session = new Session(Guid.NewGuid().ToString("N"), client);
                _sessions[session.Id] = session;

                _ = Task.Run(
                    async () =>
                    {
                        try
                        {
                            await session.ReceiveLoopAsync(HandleFrameAsync, cancellationToken);
                        }
                        catch (OperationCanceledException)
                        {
                            // Graceful shutdown path.
                        }
                        finally
                        {
                            _sessions.TryRemove(session.Id, out _);
                            await session.DisposeAsync();
                        }
                    },
                    cancellationToken);
            }
        }
        finally
        {
            _listener.Stop();
        }
    }

    private Task HandleFrameAsync(Session session, Shared.Abstractions.Net.Frame frame, CancellationToken cancellationToken)
    {
        return _dispatcher.DispatchAsync(session, frame, cancellationToken);
    }

    public async ValueTask DisposeAsync()
    {
        _listener.Stop();
        foreach (var session in _sessions.Values)
        {
            await session.DisposeAsync();
        }
    }
}
