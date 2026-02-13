namespace EclipseOriginsModern.Server.GameServer.Persistence;

public interface IAccountRepository
{
    Task<AccountRecord?> FindByUsernameAsync(string username, CancellationToken cancellationToken = default);

    Task CreateAsync(AccountRecord account, CancellationToken cancellationToken = default);

    Task UpdatePasswordAsync(
        Guid accountId,
        string passwordAlgorithm,
        int passwordIterations,
        string passwordSalt,
        string passwordHash,
        DateTimeOffset updatedAt,
        CancellationToken cancellationToken = default);
}
