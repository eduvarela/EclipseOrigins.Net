using System.Collections.Concurrent;

namespace EclipseOriginsModern.Server.GameServer.Net.Security;

public sealed class AbuseDetector
{
    private readonly ConcurrentDictionary<string, SessionAbuseState> _abuseStates = new();

    public AbuseDetector(int violationsBeforeDisconnect, TimeSpan backoffDuration)
    {
        if (violationsBeforeDisconnect <= 0) throw new ArgumentOutOfRangeException(nameof(violationsBeforeDisconnect));
        if (backoffDuration <= TimeSpan.Zero) throw new ArgumentOutOfRangeException(nameof(backoffDuration));

        ViolationsBeforeDisconnect = violationsBeforeDisconnect;
        BackoffDuration = backoffDuration;
    }

    public int ViolationsBeforeDisconnect { get; }
    public TimeSpan BackoffDuration { get; }

    public AbuseDecision Evaluate(string sessionId, bool isViolation, DateTimeOffset timestamp)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(sessionId);

        var state = _abuseStates.GetOrAdd(sessionId, _ => new SessionAbuseState());

        lock (state.Gate)
        {
            if (state.BackoffUntil.HasValue && timestamp < state.BackoffUntil.Value)
            {
                return new AbuseDecision(true, false, state.Violations, state.BackoffUntil.Value);
            }

            if (!isViolation)
            {
                state.BackoffUntil = null;
                return new AbuseDecision(false, false, state.Violations, null);
            }

            state.Violations++;
            var shouldDisconnect = state.Violations >= ViolationsBeforeDisconnect;
            if (!shouldDisconnect)
            {
                state.BackoffUntil = timestamp.Add(BackoffDuration);
            }

            return new AbuseDecision(
                isBackoffActive: !shouldDisconnect,
                disconnect: shouldDisconnect,
                violationCount: state.Violations,
                backoffUntil: state.BackoffUntil);
        }
    }

    private sealed class SessionAbuseState
    {
        public object Gate { get; } = new();
        public int Violations { get; set; }
        public DateTimeOffset? BackoffUntil { get; set; }
    }
}

public sealed record AbuseDecision(bool IsBackoffActive, bool Disconnect, int ViolationCount, DateTimeOffset? BackoffUntil);
