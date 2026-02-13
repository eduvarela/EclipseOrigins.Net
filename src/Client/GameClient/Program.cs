using EclipseOriginsModern.Shared.Protocol;

var startupPing = new PingMessage(DateTimeOffset.UtcNow);
Console.WriteLine($"GameClient bootstrap OK ({startupPing.SentAt:O})");
