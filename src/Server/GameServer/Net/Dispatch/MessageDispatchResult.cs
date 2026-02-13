namespace EclipseOriginsModern.Server.GameServer.Net.Dispatch;

public enum MessageDispatchResult
{
    Handled,
    RejectedByBackoff,
    SessionDisconnected
}
