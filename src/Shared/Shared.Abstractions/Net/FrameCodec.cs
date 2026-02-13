using System;
using System.Buffers.Binary;

namespace EclipseOrigins.Shared.Abstractions.Net;

public static class FrameCodec
{
    public const int HeaderSize = 6; // uint32 length + uint16 msgType

    public static byte[] Encode(ushort messageType, ReadOnlySpan<byte> payload)
    {
        int bodyLength = 2 + payload.Length;
        byte[] result = new byte[HeaderSize + payload.Length];
        BinaryPrimitives.WriteUInt32LittleEndian(result.AsSpan(0, 4), (uint)bodyLength);
        BinaryPrimitives.WriteUInt16LittleEndian(result.AsSpan(4, 2), messageType);
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

        uint bodyLength = BinaryPrimitives.ReadUInt32LittleEndian(buffer.Slice(0, 4));
        if (bodyLength < 2)
        {
            throw new InvalidOperationException("Invalid frame length.");
        }

        int totalLength = checked((int)bodyLength + 4);
        if (buffer.Length < totalLength)
        {
            return false;
        }

        ushort messageType = BinaryPrimitives.ReadUInt16LittleEndian(buffer.Slice(4, 2));
        byte[] payload = buffer.Slice(HeaderSize, totalLength - HeaderSize).ToArray();

        bytesConsumed = totalLength;
        frame = new Frame(messageType, payload);
        return true;
    }
}

public readonly record struct Frame(ushort MessageType, byte[] Payload);
