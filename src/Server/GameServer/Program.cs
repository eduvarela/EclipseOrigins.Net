using EclipseOriginsModern.Shared.Protocol;

var startupPing = new PingMessage(DateTimeOffset.UtcNow);
Console.WriteLine($"GameServer bootstrap OK ({startupPing.SentAt:O})");
