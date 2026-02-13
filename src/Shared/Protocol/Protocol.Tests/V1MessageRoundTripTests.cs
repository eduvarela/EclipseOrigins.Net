using EclipseOrigins.Protocol.V1;
using Google.Protobuf;

namespace Protocol.Tests;

public sealed class V1MessageRoundTripTests
{
    [Fact]
    public void HandshakeRequest_RoundTrips()
    {
        var message = new HandshakeRequest
        {
            ProtocolVersion = "1.0.0",
            ClientName = "game-client"
        };

        message.FeatureFlags.Add("movement");
        message.FeatureFlags.Add("snapshot");

        var copy = RoundTrip(message, HandshakeRequest.Parser);

        Assert.Equal("1.0.0", copy.ProtocolVersion);
        Assert.Equal(new[] { "movement", "snapshot" }, copy.FeatureFlags);
        Assert.Equal("game-client", copy.ClientName);
    }

    [Fact]
    public void HandshakeResponse_RoundTrips()
    {
        var message = new HandshakeResponse
        {
            ProtocolVersion = "1.0.0",
            Accepted = true,
            Message = "ready"
        };

        message.FeatureFlags.Add("movement");

        var copy = RoundTrip(message, HandshakeResponse.Parser);

        Assert.True(copy.Accepted);
        Assert.Equal("ready", copy.Message);
        Assert.Equal("1.0.0", copy.ProtocolVersion);
        Assert.Equal(new[] { "movement" }, copy.FeatureFlags);
    }

    [Fact]
    public void LoginRequest_RoundTrips()
    {
        var message = new LoginRequest
        {
            Metadata = CreateMetadata(),
            Username = "player-one",
            Password = "secret"
        };

        var copy = RoundTrip(message, LoginRequest.Parser);

        Assert.Equal("player-one", copy.Username);
        Assert.Equal("secret", copy.Password);
        Assert.Equal("1.0.0", copy.Metadata.ProtocolVersion);
    }

    [Fact]
    public void LoginResponse_RoundTrips()
    {
        var message = new LoginResponse
        {
            Metadata = CreateMetadata(),
            Success = true,
            SessionToken = "session-token",
            Message = "authenticated"
        };

        var copy = RoundTrip(message, LoginResponse.Parser);

        Assert.True(copy.Success);
        Assert.Equal("session-token", copy.SessionToken);
        Assert.Equal("authenticated", copy.Message);
    }

    [Fact]
    public void EnterWorldRequest_RoundTrips()
    {
        var message = new EnterWorldRequest
        {
            Metadata = CreateMetadata(),
            CharacterId = "char-001"
        };

        var copy = RoundTrip(message, EnterWorldRequest.Parser);

        Assert.Equal("char-001", copy.CharacterId);
        Assert.Equal("1.0.0", copy.Metadata.ProtocolVersion);
    }

    [Fact]
    public void EnterWorldResponse_RoundTrips()
    {
        var message = new EnterWorldResponse
        {
            Metadata = CreateMetadata(),
            Success = true,
            WorldInstanceId = "world-a",
            Message = "spawned"
        };

        var copy = RoundTrip(message, EnterWorldResponse.Parser);

        Assert.True(copy.Success);
        Assert.Equal("world-a", copy.WorldInstanceId);
        Assert.Equal("spawned", copy.Message);
    }

    [Fact]
    public void ClientInputMove_RoundTrips()
    {
        var message = new ClientInputMove
        {
            CharacterId = "char-001",
            Magnitude = 0.75f,
            ClientTick = 128,
            Direction = new Vector3
            {
                X = 1,
                Y = 0,
                Z = -1
            }
        };

        var copy = RoundTrip(message, ClientInputMove.Parser);

        Assert.Equal("char-001", copy.CharacterId);
        Assert.Equal(0.75f, copy.Magnitude);
        Assert.Equal((ulong)128, copy.ClientTick);
        Assert.Equal(1, copy.Direction.X);
        Assert.Equal(-1, copy.Direction.Z);
    }

    [Fact]
    public void ServerSnapshot_RoundTrips()
    {
        var message = new ServerSnapshot
        {
            Metadata = CreateMetadata(),
            ServerTick = 256
        };

        message.Entities.Add(new EntityState
        {
            EntityId = "npc-7",
            Position = new Vector3 { X = 10, Y = 1, Z = -3 },
            Velocity = new Vector3 { X = 0.2f, Y = 0, Z = 0.1f }
        });

        var copy = RoundTrip(message, ServerSnapshot.Parser);

        Assert.Equal((ulong)256, copy.ServerTick);
        Assert.Single(copy.Entities);
        Assert.Equal("npc-7", copy.Entities[0].EntityId);
        Assert.Equal(10, copy.Entities[0].Position.X);
        Assert.Equal(0.1f, copy.Entities[0].Velocity.Z);
        Assert.Equal("1.0.0", copy.Metadata.ProtocolVersion);
    }

    private static ProtocolMetadata CreateMetadata()
    {
        var metadata = new ProtocolMetadata
        {
            ProtocolVersion = "1.0.0"
        };

        metadata.FeatureFlags.Add("movement");
        metadata.FeatureFlags.Add("snapshot");
        return metadata;
    }

    private static T RoundTrip<T>(T message, MessageParser<T> parser)
        where T : class, IMessage<T>
    {
        var data = message.ToByteArray();
        return parser.ParseFrom(data);
    }
}
