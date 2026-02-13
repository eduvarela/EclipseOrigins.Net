namespace EclipseOriginsModern.Server.GameServer.Persistence;

public sealed record AccountRecord(
    Guid Id,
    string Username,
    string PasswordAlgorithm,
    int PasswordIterations,
    string PasswordSalt,
    string PasswordHash,
    DateTimeOffset CreatedAt,
    DateTimeOffset UpdatedAt);
