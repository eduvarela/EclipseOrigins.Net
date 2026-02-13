using EclipseOriginsModern.Server.GameServer.Core.Auth;

namespace EclipseOriginsModern.Server.GameServer.Tests;

public sealed class AuthTests
{
    [Fact]
    public void HashAndVerify_ValidPassword_ReturnsTrue()
    {
        var hasher = new PasswordHasher();
        var hash = hasher.HashPassword("hunter2");

        var verified = hasher.Verify("hunter2", hash);

        Assert.True(verified);
    }

    [Fact]
    public void HashAndVerify_InvalidPassword_ReturnsFalse()
    {
        var hasher = new PasswordHasher();
        var hash = hasher.HashPassword("hunter2");

        var verified = hasher.Verify("wrong-password", hash);

        Assert.False(verified);
    }
}
