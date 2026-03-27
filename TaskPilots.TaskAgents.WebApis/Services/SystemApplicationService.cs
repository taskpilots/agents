using TaskPilots.TaskAgents.Core;
using TaskPilots.TaskAgents.Repositories;

namespace TaskPilots.TaskAgents.WebApis.Services;

public sealed class SystemApplicationService(
    ITaskRepository taskRepository,
    IApprovalRepository approvalRepository,
    INotificationRepository notificationRepository,
    IMailboxRepository mailboxRepository)
{
    public async Task<SystemSummaryDto> GetSummaryAsync(CancellationToken cancellationToken = default)
    {
        var tasks = await taskRepository.ListAsync(cancellationToken);
        var approvals = await approvalRepository.ListAsync(cancellationToken: cancellationToken);
        var notifications = await notificationRepository.ListAsync(cancellationToken: cancellationToken);
        var mailboxMessages = await mailboxRepository.ListAsync(cancellationToken);

        return new SystemSummaryDto(
            tasks.Count,
            tasks.Count(x => x.Status is TaskStatus.Running or TaskStatus.WaitingForExecution or TaskStatus.WaitingForApproval),
            approvals.Count(x => x.Status == ApprovalStatus.Pending),
            notifications.Count,
            mailboxMessages.Count,
            tasks.Take(5).Select(x => x.ToListItem()).ToList(),
            approvals.Where(x => x.Status == ApprovalStatus.Pending).Take(5).Select(x => x.ToListItem()).ToList(),
            notifications.Take(5).Select(x => x.ToListItem()).ToList());
    }
}
