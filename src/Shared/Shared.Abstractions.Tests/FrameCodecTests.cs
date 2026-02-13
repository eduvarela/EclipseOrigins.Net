using System;
using EclipseOrigins.Shared.Abstractions.Net;
using Xunit;

namespace EclipseOrigins.Shared.Abstractions.Tests;

public class FrameCodecTests
{
    [Fact]
    public void EncodeDecode_SingleFrame_Works()
    {
        byte[] encoded = FrameCodec.Encode(10, new byte[] { 1, 2, 3 });
        bool ok = FrameCodec.TryDecode(encoded, out int consumed, out Frame frame);

        Assert.True(ok);
        Assert.Equal(encoded.Length, consumed);
        Assert.Equal((ushort)10, frame.MessageType);
        Assert.Equal(new byte[] { 1, 2, 3 }, frame.Payload);
    }

    [Fact]
    public void FrameBuffer_HandlesFragmentedReads()
    {
        byte[] encoded = FrameCodec.Encode(20, new byte[] { 9, 8, 7, 6 });
        var buffer = new FrameBuffer();

        buffer.Append(encoded.AsSpan(0, 3));
        Assert.False(buffer.TryReadFrame(out _));

        buffer.Append(encoded.AsSpan(3));
        Assert.True(buffer.TryReadFrame(out Frame frame));
        Assert.Equal((ushort)20, frame.MessageType);
        Assert.Equal(new byte[] { 9, 8, 7, 6 }, frame.Payload);
    }

    [Fact]
    public void FrameBuffer_HandlesMultipleFramesInSingleBuffer()
    {
        byte[] first = FrameCodec.Encode(1, new byte[] { 1 });
        byte[] second = FrameCodec.Encode(2, new byte[] { 2, 2 });
        byte[] aggregate = new byte[first.Length + second.Length];
        first.CopyTo(aggregate, 0);
        second.CopyTo(aggregate, first.Length);

        var buffer = new FrameBuffer();
        buffer.Append(aggregate);

        Assert.True(buffer.TryReadFrame(out Frame f1));
        Assert.True(buffer.TryReadFrame(out Frame f2));
        Assert.Equal((ushort)1, f1.MessageType);
        Assert.Equal((ushort)2, f2.MessageType);
        Assert.False(buffer.TryReadFrame(out _));
    }

    [Fact]
    public void TryDecode_InvalidLength_Throws()
    {
        byte[] invalid = new byte[] { 1, 0, 0, 0, 5, 0, 99 };
        Assert.Throws<InvalidOperationException>(() => FrameCodec.TryDecode(invalid, out _, out _));
    }
}
