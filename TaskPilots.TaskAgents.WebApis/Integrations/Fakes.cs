using TaskPilots.TaskAgents.Core;

namespace TaskPilots.TaskAgents.WebApis.Integrations;

public sealed class FakeOpenAiClient : IOpenAiClient
{
    public Task<string> GenerateRunSummaryAsync(TaskRecord task, CancellationToken cancellationToken = default)
        => Task.FromResult($"Fake agent summary for task '{task.Title}' generated at {DateTimeOffset.UtcNow:O}.");
}

public sealed class FakeMailSender : IMailSender
{
    public Task SendNotificationAsync(NotificationRecord notification, CancellationToken cancellationToken = default)
        => Task.CompletedTask;
}

public sealed class FakeMailboxPoller : IMailboxPoller
{
    public Task<IReadOnlyList<MailboxMessageRecord>> PollAsync(CancellationToken cancellationToken = default)
        => Task.FromResult<IReadOnlyList<MailboxMessageRecord>>([]);
}

public sealed class FakeWebhookIngressHandler : IWebhookIngressHandler
{
    public Task<string> HandleAsync(WebhookInboundRequest request, CancellationToken cancellationToken = default)
        => Task.FromResult($"Webhook '{request.EventName}' from '{request.Source}' accepted by fake handler.");
}
