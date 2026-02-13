namespace EclipseOriginsModern.Shared.Abstractions.Net;

public sealed class FrameBuffer
{
    private byte[] _buffer = new byte[256];
    private int _count;

    public int BufferedBytes => _count;

    public void Append(ReadOnlySpan<byte> data)
    {
        if (data.IsEmpty)
        {
            return;
        }

        EnsureCapacity(_count + data.Length);
        data.CopyTo(_buffer.AsSpan(_count));
        _count += data.Length;
    }

    public FrameDecodeStatus TryReadFrame(out Frame frame)
    {
        var status = FrameCodec.TryDecode(_buffer.AsSpan(0, _count), out frame, out var bytesConsumed);
        if (status == FrameDecodeStatus.Success)
        {
            RemovePrefix(bytesConsumed);
            return FrameDecodeStatus.Success;
        }

        if (status == FrameDecodeStatus.Malformed)
        {
            _count = 0;
        }

        return status;
    }

    private void EnsureCapacity(int requiredCapacity)
    {
        if (requiredCapacity <= _buffer.Length)
        {
            return;
        }

        var newCapacity = _buffer.Length;
        while (newCapacity < requiredCapacity)
        {
            newCapacity *= 2;
        }

        Array.Resize(ref _buffer, newCapacity);
    }

    private void RemovePrefix(int byteCount)
    {
        if (byteCount <= 0)
        {
            return;
        }

        if (byteCount >= _count)
        {
            _count = 0;
            return;
        }

        Buffer.BlockCopy(_buffer, byteCount, _buffer, 0, _count - byteCount);
        _count -= byteCount;
    }
}
