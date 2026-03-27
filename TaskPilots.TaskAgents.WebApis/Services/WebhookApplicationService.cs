using TaskPilots.TaskAgents.Core;
using TaskPilots.TaskAgents.Repositories;
using TaskPilots.TaskAgents.WebApis.Integrations;
using TaskPilots.TaskAgents.WebApis.Realtime;

namespace TaskPilots.TaskAgents.WebApis.Services;

public sealed class WebhookApplicationService(
    IEventRepository eventRepository,
    TaskProvisioningService provisioningService,
    IWebhookIngressHandler webhookIngressHandler,
    TaskApplicationService taskApplicationService,
    ITaskAgentsRealtimeNotifier realtimeNotifier)
{
    public async Task<TaskDetailDto> HandleAsync(WebhookInboundRequest request, CancellationToken cancellationToken = default)
    {
        var handlerSummary = await webhookIngressHandler.HandleAsync(request, cancellationToken);
        var task = await provisioningService.CreateTaskAsync(
            string.IsNullOrWhiteSpace(request.EventName) ? "Webhook inbound event" : request.EventName.Trim(),
            string.IsNullOrWhiteSpace(request.Payload) ? handlerSummary : request.Payload.Trim(),
            string.IsNullOrWhiteSpace(request.Source) ? "webhook" : request.Source.Trim(),
            requiresApproval: false,
            cancellationToken: cancellationToken);

        await eventRepository.AddAsync(new TaskEventRecord(
            $"event-{Guid.NewGuid():N}",
            task.TaskId,
            "webhookReceived",
            request.Source,
            handlerSummary,
            DateTimeOffset.UtcNow), cancellationToken);
        await realtimeNotifier.BroadcastRefreshAsync("tasksChanged", cancellationToken);
        return (await taskApplicationService.GetTaskAsync(task.TaskId, cancellationToken))!;
    }
}
