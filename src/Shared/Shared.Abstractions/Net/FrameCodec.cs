using System.Buffers.Binary;

namespace EclipseOriginsModern.Shared.Abstractions.Net;

public readonly record struct Frame(ushort MessageType, byte[] Payload);

public enum FrameDecodeStatus
{
    Success,
    Incomplete,
    Malformed
}

public static class FrameCodec
{
    public const int LengthPrefixSize = sizeof(uint);
    public const int MessageTypeSize = sizeof(ushort);
    public const int HeaderSize = LengthPrefixSize + MessageTypeSize;

    public static byte[] Encode(ushort messageType, ReadOnlySpan<byte> payload)
    {
        var bodyLength = checked(payload.Length + MessageTypeSize);
        var frame = new byte[checked(LengthPrefixSize + bodyLength)];

        BinaryPrimitives.WriteUInt32BigEndian(frame.AsSpan(0, LengthPrefixSize), (uint)bodyLength);
        BinaryPrimitives.WriteUInt16BigEndian(frame.AsSpan(LengthPrefixSize, MessageTypeSize), messageType);
        payload.CopyTo(frame.AsSpan(HeaderSize));

        return frame;
    }

    public static FrameDecodeStatus TryDecode(ReadOnlySpan<byte> buffer, out Frame frame, out int bytesConsumed)
    {
        frame = default;
        bytesConsumed = 0;

        if (buffer.Length < LengthPrefixSize)
        {
            return FrameDecodeStatus.Incomplete;
        }

        var bodyLength = BinaryPrimitives.ReadUInt32BigEndian(buffer[..LengthPrefixSize]);
        if (bodyLength < MessageTypeSize)
        {
            return FrameDecodeStatus.Malformed;
        }

        if (bodyLength > int.MaxValue - LengthPrefixSize)
        {
            return FrameDecodeStatus.Malformed;
        }

        var totalLength = checked((int)bodyLength + LengthPrefixSize);
        if (buffer.Length < totalLength)
        {
            return FrameDecodeStatus.Incomplete;
        }

        var messageType = BinaryPrimitives.ReadUInt16BigEndian(buffer.Slice(LengthPrefixSize, MessageTypeSize));
        var payloadLength = checked((int)bodyLength - MessageTypeSize);
        var payload = buffer.Slice(HeaderSize, payloadLength).ToArray();

        frame = new Frame(messageType, payload);
        bytesConsumed = totalLength;
        return FrameDecodeStatus.Success;
    }
}
