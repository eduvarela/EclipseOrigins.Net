using System.Security.Cryptography;

namespace EclipseOriginsModern.Server.GameServer.Core.Auth;

public sealed class PasswordHasher
{
    public const string CurrentAlgorithm = "PBKDF2-SHA256";

    private readonly int _iterations;
    private readonly int _saltSize;
    private readonly int _keySize;

    public PasswordHasher(int iterations = 120_000, int saltSize = 16, int keySize = 32)
    {
        if (iterations <= 0) throw new ArgumentOutOfRangeException(nameof(iterations));
        if (saltSize < 8) throw new ArgumentOutOfRangeException(nameof(saltSize));
        if (keySize < 16) throw new ArgumentOutOfRangeException(nameof(keySize));

        _iterations = iterations;
        _saltSize = saltSize;
        _keySize = keySize;
    }

    public PasswordHashRecord HashPassword(string password)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(password);

        var salt = RandomNumberGenerator.GetBytes(_saltSize);
        var hash = Rfc2898DeriveBytes.Pbkdf2(
            password,
            salt,
            _iterations,
            HashAlgorithmName.SHA256,
            _keySize);

        return new PasswordHashRecord(
            CurrentAlgorithm,
            _iterations,
            Convert.ToBase64String(salt),
            Convert.ToBase64String(hash));
    }

    public bool Verify(string password, PasswordHashRecord stored)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(password);
        ArgumentNullException.ThrowIfNull(stored);

        if (!string.Equals(stored.Algorithm, CurrentAlgorithm, StringComparison.Ordinal))
        {
            return false;
        }

        byte[] salt;
        byte[] expectedHash;
        try
        {
            salt = Convert.FromBase64String(stored.Salt);
            expectedHash = Convert.FromBase64String(stored.Hash);
        }
        catch (FormatException)
        {
            return false;
        }

        var computedHash = Rfc2898DeriveBytes.Pbkdf2(
            password,
            salt,
            stored.Iterations,
            HashAlgorithmName.SHA256,
            expectedHash.Length);

        return CryptographicOperations.FixedTimeEquals(computedHash, expectedHash);
    }
}
