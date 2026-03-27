using TaskPilots.TaskAgents.Core;
using TaskPilots.TaskAgents.Repositories;
using TaskPilots.TaskAgents.WebApis.Integrations;
using TaskPilots.TaskAgents.WebApis.Realtime;

namespace TaskPilots.TaskAgents.WebApis.Services;

public sealed class ApprovalApplicationService(
    ITaskRepository taskRepository,
    IEventRepository eventRepository,
    IRunRepository runRepository,
    IApprovalRepository approvalRepository,
    INotificationRepository notificationRepository,
    IMailSender mailSender,
    ITaskAgentsRealtimeNotifier realtimeNotifier)
{
    public async Task<IReadOnlyList<ApprovalListItemDto>> GetApprovalsAsync(CancellationToken cancellationToken = default)
        => (await approvalRepository.ListAsync(cancellationToken: cancellationToken)).Select(x => x.ToListItem()).ToList();

    public Task<ApprovalListItemDto?> ApproveAsync(string approvalId, ApprovalDecisionRequest request, CancellationToken cancellationToken = default)
        => ResolveAsync(approvalId, request, ApprovalStatus.Approved, RunStatus.Completed, TaskStatus.Completed, "approvalApproved", cancellationToken);

    public Task<ApprovalListItemDto?> RejectAsync(string approvalId, ApprovalDecisionRequest request, CancellationToken cancellationToken = default)
        => ResolveAsync(approvalId, request, ApprovalStatus.Rejected, RunStatus.Failed, TaskStatus.Failed, "approvalRejected", cancellationToken);

    private async Task<ApprovalListItemDto?> ResolveAsync(
        string approvalId,
        ApprovalDecisionRequest request,
        ApprovalStatus approvalStatus,
        RunStatus runStatus,
        TaskStatus taskStatus,
        string eventType,
        CancellationToken cancellationToken)
    {
        var approval = await approvalRepository.GetAsync(approvalId, cancellationToken);
        if (approval is null)
        {
            return null;
        }

        var nowUtc = DateTimeOffset.UtcNow;
        var resolvedApproval = approval with
        {
            Status = approvalStatus,
            DecisionNote = request.Note.Trim(),
            ResolvedAtUtc = nowUtc,
        };
        await approvalRepository.UpdateAsync(resolvedApproval, cancellationToken);

        if (!string.IsNullOrWhiteSpace(approval.RunId))
        {
            var run = await runRepository.GetAsync(approval.RunId, cancellationToken);
            if (run is not null)
            {
                await runRepository.UpdateAsync(run with
                {
                    Status = runStatus,
                    CompletedAtUtc = nowUtc,
                    CurrentStep = approvalStatus == ApprovalStatus.Approved ? "completed" : "rejected",
                    OutputSummary = approvalStatus == ApprovalStatus.Approved ? "Approved and marked complete." : "Rejected by reviewer.",
                    WaitingReason = null,
                }, cancellationToken);
            }
        }

        var task = await taskRepository.GetAsync(approval.TaskId, cancellationToken);
        if (task is not null)
        {
            await taskRepository.UpdateAsync(task with
            {
                Status = taskStatus,
                UpdatedAtUtc = nowUtc,
                NextExecutionAtUtc = null,
                LastError = approvalStatus == ApprovalStatus.Rejected ? "Approval rejected." : null,
            }, cancellationToken);
        }

        await eventRepository.AddAsync(new TaskEventRecord(
            $"event-{Guid.NewGuid():N}",
            approval.TaskId,
            eventType,
            "approval",
            string.IsNullOrWhiteSpace(request.Note) ? approvalStatus.ToString() : request.Note.Trim(),
            nowUtc), cancellationToken);

        var notification = new NotificationRecord(
            $"notification-{Guid.NewGuid():N}",
            approval.TaskId,
            approvalStatus == ApprovalStatus.Approved ? "approvalApproved" : "approvalRejected",
            "in_app",
            NotificationStatus.Sent,
            approvalStatus == ApprovalStatus.Approved ? "Approval approved" : "Approval rejected",
            string.IsNullOrWhiteSpace(request.Note) ? "No note supplied." : request.Note.Trim(),
            nowUtc,
            nowUtc);
        await notificationRepository.AddAsync(notification, cancellationToken);
        await mailSender.SendNotificationAsync(notification, cancellationToken);
        await realtimeNotifier.BroadcastRefreshAsync("approvalsChanged", cancellationToken);
        return resolvedApproval.ToListItem();
    }
}
