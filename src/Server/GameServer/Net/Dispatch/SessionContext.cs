namespace EclipseOriginsModern.Server.GameServer.Net.Dispatch;

public interface ISessionContext
{
    string SessionId { get; }
    Task DisconnectAsync(string reason, CancellationToken cancellationToken = default);
}
