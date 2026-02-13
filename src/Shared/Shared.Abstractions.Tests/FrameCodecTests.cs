using EclipseOriginsModern.Shared.Abstractions.Net;

namespace Shared.Abstractions.Tests;

public sealed class FrameCodecTests
{
    [Fact]
    public void SingleCompleteFrame_IsDecoded()
    {
        var payload = new byte[] { 0x10, 0x20, 0x30 };
        var frameBytes = FrameCodec.Encode(7, payload);

        var buffer = new FrameBuffer();
        buffer.Append(frameBytes);

        var status = buffer.TryReadFrame(out var frame);

        Assert.Equal(FrameDecodeStatus.Success, status);
        Assert.Equal((ushort)7, frame.MessageType);
        Assert.Equal(payload, frame.Payload);
        Assert.Equal(0, buffer.BufferedBytes);
    }

    [Fact]
    public void FragmentedFrameAcrossReceives_WaitsForCompletion()
    {
        var payload = new byte[] { 0xAB, 0xCD, 0xEF, 0x11 };
        var frameBytes = FrameCodec.Encode(100, payload);

        var buffer = new FrameBuffer();
        buffer.Append(frameBytes.AsSpan(0, 3));

        var firstStatus = buffer.TryReadFrame(out _);
        Assert.Equal(FrameDecodeStatus.Incomplete, firstStatus);

        buffer.Append(frameBytes.AsSpan(3, 2));
        var secondStatus = buffer.TryReadFrame(out _);
        Assert.Equal(FrameDecodeStatus.Incomplete, secondStatus);

        buffer.Append(frameBytes.AsSpan(5));
        var finalStatus = buffer.TryReadFrame(out var frame);

        Assert.Equal(FrameDecodeStatus.Success, finalStatus);
        Assert.Equal((ushort)100, frame.MessageType);
        Assert.Equal(payload, frame.Payload);
    }

    [Fact]
    public void MultipleFramesInOneReceive_AreReadSequentially()
    {
        var first = FrameCodec.Encode(1, new byte[] { 0x01 });
        var second = FrameCodec.Encode(2, new byte[] { 0xAA, 0xBB });

        var combined = new byte[first.Length + second.Length];
        Buffer.BlockCopy(first, 0, combined, 0, first.Length);
        Buffer.BlockCopy(second, 0, combined, first.Length, second.Length);

        var buffer = new FrameBuffer();
        buffer.Append(combined);

        var firstStatus = buffer.TryReadFrame(out var firstFrame);
        var secondStatus = buffer.TryReadFrame(out var secondFrame);

        Assert.Equal(FrameDecodeStatus.Success, firstStatus);
        Assert.Equal((ushort)1, firstFrame.MessageType);
        Assert.Equal(new byte[] { 0x01 }, firstFrame.Payload);

        Assert.Equal(FrameDecodeStatus.Success, secondStatus);
        Assert.Equal((ushort)2, secondFrame.MessageType);
        Assert.Equal(new byte[] { 0xAA, 0xBB }, secondFrame.Payload);
        Assert.Equal(0, buffer.BufferedBytes);
    }

    [Fact]
    public void MalformedLengthOrMissingMsgType_UsesSafeFailurePath()
    {
        var malformed = new byte[]
        {
            0x00, 0x00, 0x00, 0x01,
            0x99
        };

        var decodeStatus = FrameCodec.TryDecode(malformed, out _, out _);
        Assert.Equal(FrameDecodeStatus.Malformed, decodeStatus);

        var buffer = new FrameBuffer();
        buffer.Append(malformed);

        var malformedStatus = buffer.TryReadFrame(out _);
        Assert.Equal(FrameDecodeStatus.Malformed, malformedStatus);
        Assert.Equal(0, buffer.BufferedBytes);

        var valid = FrameCodec.Encode(42, new byte[] { 0xFE });
        buffer.Append(valid);

        var recoveryStatus = buffer.TryReadFrame(out var recovered);
        Assert.Equal(FrameDecodeStatus.Success, recoveryStatus);
        Assert.Equal((ushort)42, recovered.MessageType);
        Assert.Equal(new byte[] { 0xFE }, recovered.Payload);
    }
}
