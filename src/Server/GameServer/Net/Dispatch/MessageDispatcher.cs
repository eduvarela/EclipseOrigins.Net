using EclipseOriginsModern.Server.GameServer.Net.Security;

namespace EclipseOriginsModern.Server.GameServer.Net.Dispatch;

public sealed class MessageDispatcher
{
    private readonly RateLimiter _rateLimiter;
    private readonly AbuseDetector _abuseDetector;

    public MessageDispatcher(RateLimiter rateLimiter, AbuseDetector abuseDetector)
    {
        _rateLimiter = rateLimiter;
        _abuseDetector = abuseDetector;
    }

    public async Task<MessageDispatchResult> DispatchAsync(
        ISessionContext session,
        Func<CancellationToken, Task> handler,
        CancellationToken cancellationToken = default)
    {
        var now = DateTimeOffset.UtcNow;
        var rateLimitDecision = _rateLimiter.Evaluate(session.SessionId, now);
        var abuseDecision = _abuseDetector.Evaluate(session.SessionId, !rateLimitDecision.Allowed, now);

        if (abuseDecision.Disconnect)
        {
            await session.DisconnectAsync("Disconnected for abusive traffic.", cancellationToken);
            return MessageDispatchResult.SessionDisconnected;
        }

        if (abuseDecision.IsBackoffActive)
        {
            return MessageDispatchResult.RejectedByBackoff;
        }

        await handler(cancellationToken);
        return MessageDispatchResult.Handled;
    }
}
