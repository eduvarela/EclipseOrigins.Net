using System.Collections.Concurrent;

namespace EclipseOriginsModern.Server.GameServer.Net.Security;

public sealed class RateLimiter
{
    private readonly ConcurrentDictionary<string, SlidingWindowCounter> _sessionCounters = new();

    public RateLimiter(int maxMessagesPerWindow, TimeSpan window)
    {
        if (maxMessagesPerWindow <= 0) throw new ArgumentOutOfRangeException(nameof(maxMessagesPerWindow));
        if (window <= TimeSpan.Zero) throw new ArgumentOutOfRangeException(nameof(window));

        MaxMessagesPerWindow = maxMessagesPerWindow;
        Window = window;
    }

    public int MaxMessagesPerWindow { get; }
    public TimeSpan Window { get; }

    public RateLimitDecision Evaluate(string sessionId, DateTimeOffset timestamp)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(sessionId);

        var counter = _sessionCounters.GetOrAdd(sessionId, _ => new SlidingWindowCounter(Window));
        var count = counter.IncrementAndGetCount(timestamp);
        var allowed = count <= MaxMessagesPerWindow;

        return new RateLimitDecision(allowed, count, MaxMessagesPerWindow, counter.WindowStart);
    }

    private sealed class SlidingWindowCounter
    {
        private readonly object _gate = new();
        private readonly TimeSpan _window;
        private int _count;

        public SlidingWindowCounter(TimeSpan window)
        {
            _window = window;
            WindowStart = DateTimeOffset.MinValue;
        }

        public DateTimeOffset WindowStart { get; private set; }

        public int IncrementAndGetCount(DateTimeOffset now)
        {
            lock (_gate)
            {
                if (WindowStart == DateTimeOffset.MinValue || now - WindowStart >= _window)
                {
                    WindowStart = now;
                    _count = 0;
                }

                _count++;
                return _count;
            }
        }
    }
}

public sealed record RateLimitDecision(bool Allowed, int CurrentCount, int Limit, DateTimeOffset WindowStart);
