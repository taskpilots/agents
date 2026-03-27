using Microsoft.AspNetCore.SignalR;

namespace TaskPilots.TaskAgents.WebApis.Realtime;

public sealed class TaskAgentsHub : Hub
{
    public Task SubscribeToWorkspace(string workspace)
        => Groups.AddToGroupAsync(Context.ConnectionId, workspace);
}
