using System;
using System.Collections.Generic;

namespace EclipseOrigins.Shared.Abstractions.Net;

public sealed class FrameBuffer
{
    private readonly List<byte> _buffer = new();

    public void Append(ReadOnlySpan<byte> data)
    {
        foreach (byte b in data)
        {
            _buffer.Add(b);
        }
    }

    public bool TryReadFrame(out Frame frame)
    {
        frame = default;
        if (_buffer.Count == 0)
        {
            return false;
        }

        byte[] current = _buffer.ToArray();
        if (!FrameCodec.TryDecode(current, out int consumed, out frame))
        {
            return false;
        }

        _buffer.RemoveRange(0, consumed);
        return true;
    }
}
