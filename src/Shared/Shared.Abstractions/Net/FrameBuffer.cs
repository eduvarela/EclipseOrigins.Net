namespace EclipseOriginsModern.Shared.Abstractions.Net;

public sealed class FrameBuffer
{
    private readonly List<byte> _buffer = new();

    public int BufferedBytes => _buffer.Count;

    public void Append(ReadOnlySpan<byte> data)
    {
        if (data.IsEmpty)
        {
            return;
        }

        _buffer.AddRange(data.ToArray());
    }

    public bool TryReadFrame(out Frame frame)
    {
        frame = default;
        if (_buffer.Count == 0)
        {
            return false;
        }

        var snapshot = _buffer.ToArray();
        if (!FrameCodec.TryDecode(snapshot, out var consumed, out frame))
        {
            return false;
        }

        _buffer.RemoveRange(0, consumed);
        return true;
    }
}
