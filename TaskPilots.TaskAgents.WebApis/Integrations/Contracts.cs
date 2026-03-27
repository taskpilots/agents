using TaskPilots.TaskAgents.Core;

namespace TaskPilots.TaskAgents.WebApis.Integrations;

public interface IOpenAiClient
{
    Task<string> GenerateRunSummaryAsync(TaskRecord task, CancellationToken cancellationToken = default);
}

public interface IMailSender
{
    Task SendNotificationAsync(NotificationRecord notification, CancellationToken cancellationToken = default);
}

public interface IMailboxPoller
{
    Task<IReadOnlyList<MailboxMessageRecord>> PollAsync(CancellationToken cancellationToken = default);
}

public interface IWebhookIngressHandler
{
    Task<string> HandleAsync(WebhookInboundRequest request, CancellationToken cancellationToken = default);
}
