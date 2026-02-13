using System.Net.Sockets;
using EclipseOriginsModern.Shared.Abstractions.Net;

namespace EclipseOriginsModern.Server.GameServer.Net.Sessions;

public sealed class Session : IAsyncDisposable
{
    private readonly TcpClient _client;
    private readonly NetworkStream _stream;
    private readonly FrameBuffer _frameBuffer = new();

    public Session(string id, TcpClient client)
    {
        Id = id;
        _client = client;
        _stream = client.GetStream();
    }

    public string Id { get; }
    public bool IsAuthenticated { get; private set; }

    public void MarkAuthenticated() => IsAuthenticated = true;

    public async Task SendAsync(ushort messageType, ReadOnlyMemory<byte> payload, CancellationToken cancellationToken)
    {
        var encoded = FrameCodec.Encode(messageType, payload.Span);
        await _stream.WriteAsync(encoded, cancellationToken);
    }

    public async Task ReceiveLoopAsync(
        Func<Session, Frame, CancellationToken, Task> onFrameReceived,
        CancellationToken cancellationToken)
    {
        var recv = new byte[8 * 1024];

        while (!cancellationToken.IsCancellationRequested && _client.Connected)
        {
            var bytesRead = await _stream.ReadAsync(recv, cancellationToken);
            if (bytesRead == 0)
            {
                break;
            }

            _frameBuffer.Append(recv.AsSpan(0, bytesRead));
            while (_frameBuffer.TryReadFrame(out var frame))
            {
                await onFrameReceived(this, frame, cancellationToken);
            }
        }
    }

    public async ValueTask DisposeAsync()
    {
        _stream.Close();
        _client.Close();
        await Task.CompletedTask;
    }
}
