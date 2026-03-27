using TaskPilots.TaskAgents.Core;
using TaskPilots.TaskAgents.Repositories;
using TaskPilots.TaskAgents.WebApis.Realtime;

namespace TaskPilots.TaskAgents.WebApis.Services;

public sealed class MailboxApplicationService(
    IMailboxRepository mailboxRepository,
    TaskProvisioningService provisioningService,
    ITaskAgentsRealtimeNotifier realtimeNotifier)
{
    public async Task<IReadOnlyList<MailboxMessageListItemDto>> GetMessagesAsync(CancellationToken cancellationToken = default)
        => (await mailboxRepository.ListAsync(cancellationToken)).Select(x => x.ToListItem()).ToList();

    public async Task<MailboxMessageListItemDto> SimulateIngressAsync(SimulateMailboxIngestRequest request, CancellationToken cancellationToken = default)
    {
        var title = string.IsNullOrWhiteSpace(request.Subject) ? "Mailbox task" : request.Subject.Trim();
        var task = await provisioningService.CreateTaskAsync(
            title,
            string.IsNullOrWhiteSpace(request.Body) ? "Synthetic mailbox ingestion." : request.Body.Trim(),
            "mailbox",
            requiresApproval: false,
            cancellationToken: cancellationToken);

        var message = new MailboxMessageRecord(
            $"mail-{Guid.NewGuid():N}",
            task.TaskId,
            title,
            string.IsNullOrWhiteSpace(request.FromAddress) ? "unknown@example.local" : request.FromAddress.Trim(),
            "inbound",
            MailboxMessageStatus.LinkedToTask,
            string.IsNullOrWhiteSpace(request.Body) ? "Synthetic mailbox ingestion." : request.Body.Trim(),
            DateTimeOffset.UtcNow);
        await mailboxRepository.AddAsync(message, cancellationToken);
        await realtimeNotifier.BroadcastRefreshAsync("mailboxChanged", cancellationToken);
        return message.ToListItem();
    }
}
