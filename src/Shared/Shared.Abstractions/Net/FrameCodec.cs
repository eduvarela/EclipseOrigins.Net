using System.Buffers.Binary;

namespace EclipseOriginsModern.Shared.Abstractions.Net;

public static class FrameCodec
{
    public const int HeaderSize = 6; // uint32 length + uint16 msgType

    public static byte[] Encode(ushort messageType, ReadOnlySpan<byte> payload)
    {
        var bodyLength = checked((uint)(2 + payload.Length));
        var result = new byte[HeaderSize + payload.Length];
        BinaryPrimitives.WriteUInt32BigEndian(result.AsSpan(0, 4), bodyLength);
        BinaryPrimitives.WriteUInt16BigEndian(result.AsSpan(4, 2), messageType);
        payload.CopyTo(result.AsSpan(HeaderSize));
        return result;
    }

    public static bool TryDecode(ReadOnlySpan<byte> buffer, out int bytesConsumed, out Frame frame)
    {
        bytesConsumed = 0;
        frame = default;

        if (buffer.Length < HeaderSize)
        {
            return false;
        }

        var bodyLength = BinaryPrimitives.ReadUInt32BigEndian(buffer[..4]);
        if (bodyLength < 2)
        {
            throw new InvalidOperationException("Invalid frame body length.");
        }

        var totalLength = checked((int)bodyLength + 4);
        if (buffer.Length < totalLength)
        {
            return false;
        }

        var messageType = BinaryPrimitives.ReadUInt16BigEndian(buffer.Slice(4, 2));
        var payload = buffer.Slice(HeaderSize, totalLength - HeaderSize).ToArray();
        bytesConsumed = totalLength;
        frame = new Frame(messageType, payload);
        return true;
    }
}

public readonly record struct Frame(ushort MessageType, byte[] Payload);
