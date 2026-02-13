namespace EclipseOriginsModern.Server.GameServer.Core.Auth;

public sealed record PasswordHashRecord(
    string Algorithm,
    int Iterations,
    string Salt,
    string Hash);
