using EclipseOriginsModern.Server.GameServer.Net.Dispatch;
using EclipseOriginsModern.Server.GameServer.Net.Security;
using EclipseOriginsModern.Shared.Protocol;

var startupPing = new PingMessage(DateTimeOffset.UtcNow);
Console.WriteLine($"GameServer bootstrap OK ({startupPing.SentAt:O})");

var dispatcher = new MessageDispatcher(
    new RateLimiter(maxMessagesPerWindow: 30, window: TimeSpan.FromSeconds(1)),
    new AbuseDetector(violationsBeforeDisconnect: 5, backoffDuration: TimeSpan.FromSeconds(2)));

// Example server wiring: all inbound messages should be routed through this dispatcher.
_ = dispatcher;

