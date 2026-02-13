using EclipseOrigins.Shared.Protocol.V1;
using Xunit;

namespace EclipseOrigins.Shared.Protocol.Tests;

public class ProtocolRoundTripTests
{
    [Fact]
    public void HandshakeRequest_RoundTrip()
    {
        var message = new HandshakeRequest
        {
            ProtocolVersion = 1,
            ClientName = "test-client",
            FeatureFlags = new FeatureFlags { Values = { "base" } }
        };

        HandshakeRequest clone = HandshakeRequest.Parser.ParseFrom(message.ToByteArray());
        Assert.Equal(message.ProtocolVersion, clone.ProtocolVersion);
        Assert.Equal(message.ClientName, clone.ClientName);
        Assert.Contains("base", clone.FeatureFlags.Values);
    }

    [Fact]
    public void LoginRequest_RoundTrip()
    {
        var message = new LoginRequest { Username = "u", Password = "p" };
        LoginRequest clone = LoginRequest.Parser.ParseFrom(message.ToByteArray());
        Assert.Equal("u", clone.Username);
        Assert.Equal("p", clone.Password);
    }

    [Fact]
    public void LoginResponse_RoundTrip()
    {
        var message = new LoginResponse { Success = true, SessionToken = "token" };
        LoginResponse clone = LoginResponse.Parser.ParseFrom(message.ToByteArray());
        Assert.True(clone.Success);
        Assert.Equal("token", clone.SessionToken);
    }

    [Fact]
    public void EnterWorldRequest_RoundTrip()
    {
        var message = new EnterWorldRequest { SessionToken = "t", CharacterId = 42 };
        EnterWorldRequest clone = EnterWorldRequest.Parser.ParseFrom(message.ToByteArray());
        Assert.Equal((ulong)42, clone.CharacterId);
    }

    [Fact]
    public void EnterWorldResponse_RoundTrip()
    {
        var message = new EnterWorldResponse
        {
            Success = true,
            CharacterId = 42,
            WorldInstanceId = 99,
            SpawnPosition = new Vector2 { X = 10, Y = 20 }
        };

        EnterWorldResponse clone = EnterWorldResponse.Parser.ParseFrom(message.ToByteArray());
        Assert.True(clone.Success);
        Assert.Equal((ulong)99, clone.WorldInstanceId);
        Assert.Equal(10, clone.SpawnPosition.X);
    }

    [Fact]
    public void ClientInputMove_RoundTrip()
    {
        var message = new ClientInputMove { ClientTick = 2, MoveX = 1, MoveY = -1, IsRunning = true };
        ClientInputMove clone = ClientInputMove.Parser.ParseFrom(message.ToByteArray());
        Assert.True(clone.IsRunning);
        Assert.Equal(-1, clone.MoveY);
    }

    [Fact]
    public void ServerSnapshot_RoundTrip()
    {
        var message = new ServerSnapshot
        {
            ServerTick = 100,
            Entities =
            {
                new EntityState { EntityId = 1, Position = new Vector2 { X = 3, Y = 4 } }
            }
        };

        ServerSnapshot clone = ServerSnapshot.Parser.ParseFrom(message.ToByteArray());
        Assert.Single(clone.Entities);
        Assert.Equal((ulong)1, clone.Entities[0].EntityId);
        Assert.Equal(4, clone.Entities[0].Position.Y);
    }
}
