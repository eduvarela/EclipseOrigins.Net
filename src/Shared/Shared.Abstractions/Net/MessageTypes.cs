namespace EclipseOriginsModern.Shared.Abstractions.Net;

public static class MessageTypes
{
    public const ushort HandshakeRequest = 1;
    public const ushort HandshakeResponse = 2;
    public const ushort LoginRequest = 3;
    public const ushort LoginResponse = 4;
    public const ushort EnterWorldRequest = 5;
    public const ushort EnterWorldResponse = 6;
    public const ushort ClientInputMove = 7;
    public const ushort ServerSnapshot = 8;
}
