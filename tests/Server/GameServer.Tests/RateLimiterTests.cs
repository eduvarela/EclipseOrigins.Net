using EclipseOriginsModern.Server.GameServer.Net.Security;

namespace EclipseOriginsModern.Server.GameServer.Tests;

public sealed class RateLimiterTests
{
    [Fact]
    public void Evaluate_AboveThreshold_IsRejected()
    {
        var limiter = new RateLimiter(maxMessagesPerWindow: 2, TimeSpan.FromSeconds(1));
        var now = DateTimeOffset.UtcNow;

        Assert.True(limiter.Evaluate("s1", now).Allowed);
        Assert.True(limiter.Evaluate("s1", now).Allowed);

        var third = limiter.Evaluate("s1", now);

        Assert.False(third.Allowed);
        Assert.Equal(3, third.CurrentCount);
    }

    [Fact]
    public void AbuseDetector_TriggersBackoffThenDisconnect()
    {
        var detector = new AbuseDetector(violationsBeforeDisconnect: 3, backoffDuration: TimeSpan.FromSeconds(2));
        var now = DateTimeOffset.UtcNow;

        var firstViolation = detector.Evaluate("s1", isViolation: true, now);
        Assert.True(firstViolation.IsBackoffActive);
        Assert.False(firstViolation.Disconnect);

        var duringBackoff = detector.Evaluate("s1", isViolation: false, now.AddSeconds(1));
        Assert.True(duringBackoff.IsBackoffActive);

        var secondViolation = detector.Evaluate("s1", isViolation: true, now.AddSeconds(3));
        Assert.True(secondViolation.IsBackoffActive);
        Assert.False(secondViolation.Disconnect);

        var thirdViolation = detector.Evaluate("s1", isViolation: true, now.AddSeconds(6));
        Assert.True(thirdViolation.Disconnect);
        Assert.Equal(3, thirdViolation.ViolationCount);
    }
}
