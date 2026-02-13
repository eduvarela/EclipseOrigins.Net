using EclipseOriginsModern.Shared.Abstractions;

namespace EclipseOriginsModern.Shared.Protocol;

public sealed record PingMessage(DateTimeOffset SentAt) : IGameMessage;
