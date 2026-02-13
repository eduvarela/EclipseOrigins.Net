using System.Net;
using EclipseOrigins.Protocol.V1;
using EclipseOriginsModern.Server.GameServer.Net.Pipeline;
using EclipseOriginsModern.Server.GameServer.Net.Tcp;
using EclipseOriginsModern.Shared.Abstractions.Net;
using Google.Protobuf;

var dispatcher = new MessageDispatcher();

dispatcher.RegisterHandler(
    MessageTypes.HandshakeRequest,
    async (session, payload, cancellationToken) =>
    {
        var request = HandshakeRequest.Parser.ParseFrom(payload.Span);
        var response = new HandshakeResponse
        {
            Accepted = true,
            Message = $"Welcome {request.ClientName}"
        };

        response.Metadata = new ProtocolMetadata
        {
            ProtocolVersion = request.Metadata.ProtocolVersion,
        };
        response.Metadata.FeatureFlags.Add(request.Metadata.FeatureFlags);

        await session.SendAsync(MessageTypes.HandshakeResponse, response.ToByteArray(), cancellationToken);
    });

var endPoint = new IPEndPoint(IPAddress.Any, 7777);
await using var server = new TcpServer(endPoint, dispatcher);

Console.WriteLine($"GameServer listening on {endPoint}.");
await server.RunAsync(CancellationToken.None);
