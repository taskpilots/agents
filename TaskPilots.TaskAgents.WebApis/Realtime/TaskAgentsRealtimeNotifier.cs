using Microsoft.AspNetCore.SignalR;

namespace TaskPilots.TaskAgents.WebApis.Realtime;

public interface ITaskAgentsRealtimeNotifier
{
    Task BroadcastRefreshAsync(string message, CancellationToken cancellationToken = default);
}

public sealed class TaskAgentsRealtimeNotifier(IHubContext<TaskAgentsHub> hubContext) : ITaskAgentsRealtimeNotifier
{
    public Task BroadcastRefreshAsync(string message, CancellationToken cancellationToken = default)
        => hubContext.Clients.Group("dashboard")
            .SendAsync("onNotification", new RealtimeNotificationPayload(message, DateTimeOffset.UtcNow), cancellationToken);
}
