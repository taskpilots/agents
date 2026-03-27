using TaskPilots.TaskAgents.Core;

namespace TaskPilots.TaskAgents.WebApis.Services;

internal static class Mappings
{
    public static TaskListItemDto ToListItem(this TaskRecord task)
        => new(
            task.TaskId,
            task.Title,
            task.Source,
            task.Status.ToString(),
            task.Priority,
            task.RequiresApproval,
            task.CreatedAtUtc,
            task.UpdatedAtUtc);

    public static TaskEventDto ToDto(this TaskEventRecord taskEvent)
        => new(
            taskEvent.EventId,
            taskEvent.TaskId,
            taskEvent.EventType,
            taskEvent.Source,
            taskEvent.PayloadSummary,
            taskEvent.CreatedAtUtc);

    public static AgentRunListItemDto ToListItem(this AgentRunRecord run)
        => new(
            run.RunId,
            run.TaskId,
            run.Kind,
            run.Status.ToString(),
            run.Summary,
            run.RequestedAtUtc,
            run.StartedAtUtc,
            run.CompletedAtUtc,
            run.WaitingReason);

    public static AgentRunDetailDto ToDetail(
        this AgentRunRecord run,
        IReadOnlyList<RunCheckpointRecord> checkpoints,
        IReadOnlyList<RunExecutionPathEntryRecord> executionPath)
        => new(
            run.RunId,
            run.TaskId,
            run.Kind,
            run.Status.ToString(),
            run.Summary,
            run.OutputSummary,
            run.CurrentStep,
            run.RequestedAtUtc,
            run.StartedAtUtc,
            run.CompletedAtUtc,
            checkpoints.Select(x => new RunCheckpointDto(x.CheckpointId, x.RunId, x.StepName, x.Snapshot, x.CreatedAtUtc)).ToList(),
            executionPath.Select(x => new RunExecutionPathEntryDto(x.EntryId, x.RunId, x.StepName, x.Detail, x.CreatedAtUtc)).ToList());

    public static ApprovalListItemDto ToListItem(this ApprovalRecord approval)
        => new(
            approval.ApprovalId,
            approval.TaskId,
            approval.RunId,
            approval.Title,
            approval.Status.ToString(),
            approval.RiskLevel,
            approval.RequestedBy,
            approval.DecisionNote,
            approval.RequestedAtUtc,
            approval.ResolvedAtUtc);

    public static NotificationListItemDto ToListItem(this NotificationRecord notification)
        => new(
            notification.NotificationId,
            notification.TaskId,
            notification.Kind,
            notification.Channel,
            notification.Status.ToString(),
            notification.Title,
            notification.Message,
            notification.CreatedAtUtc,
            notification.SentAtUtc);

    public static MailboxMessageListItemDto ToListItem(this MailboxMessageRecord message)
        => new(
            message.MessageId,
            message.TaskId,
            message.Subject,
            message.FromAddress,
            message.Direction,
            message.Status.ToString(),
            message.Summary,
            message.ReceivedAtUtc);
}
