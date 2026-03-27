namespace TaskPilots.TaskAgents.Core;

public sealed record SystemSummaryDto(
    int TotalTaskCount,
    int RunningTaskCount,
    int PendingApprovalCount,
    int NotificationCount,
    int MailboxMessageCount,
    IReadOnlyList<TaskListItemDto> RecentTasks,
    IReadOnlyList<ApprovalListItemDto> PendingApprovals,
    IReadOnlyList<NotificationListItemDto> RecentNotifications);

public sealed record TaskListItemDto(
    string TaskId,
    string Title,
    string Source,
    string Status,
    string Priority,
    bool RequiresApproval,
    DateTimeOffset CreatedAtUtc,
    DateTimeOffset UpdatedAtUtc);

public sealed record TaskDetailDto(
    string TaskId,
    string Title,
    string Source,
    string Status,
    string Summary,
    string Priority,
    bool RequiresApproval,
    DateTimeOffset CreatedAtUtc,
    DateTimeOffset UpdatedAtUtc,
    IReadOnlyList<TaskEventDto> Events,
    IReadOnlyList<AgentRunListItemDto> Runs,
    IReadOnlyList<ApprovalListItemDto> Approvals);

public sealed record TaskEventDto(
    string EventId,
    string? TaskId,
    string EventType,
    string Source,
    string PayloadSummary,
    DateTimeOffset CreatedAtUtc);

public sealed record AgentRunListItemDto(
    string RunId,
    string TaskId,
    string Kind,
    string Status,
    string Summary,
    DateTimeOffset RequestedAtUtc,
    DateTimeOffset? StartedAtUtc,
    DateTimeOffset? CompletedAtUtc,
    string? WaitingReason);

public sealed record AgentRunDetailDto(
    string RunId,
    string TaskId,
    string Kind,
    string Status,
    string Summary,
    string? OutputSummary,
    string? CurrentStep,
    DateTimeOffset RequestedAtUtc,
    DateTimeOffset? StartedAtUtc,
    DateTimeOffset? CompletedAtUtc,
    IReadOnlyList<RunCheckpointDto> Checkpoints,
    IReadOnlyList<RunExecutionPathEntryDto> ExecutionPath);

public sealed record RunCheckpointDto(
    string CheckpointId,
    string RunId,
    string StepName,
    string Snapshot,
    DateTimeOffset CreatedAtUtc);

public sealed record RunExecutionPathEntryDto(
    string EntryId,
    string RunId,
    string StepName,
    string Detail,
    DateTimeOffset CreatedAtUtc);

public sealed record ApprovalListItemDto(
    string ApprovalId,
    string TaskId,
    string? RunId,
    string Title,
    string Status,
    string RiskLevel,
    string RequestedBy,
    string? DecisionNote,
    DateTimeOffset RequestedAtUtc,
    DateTimeOffset? ResolvedAtUtc);

public sealed record NotificationListItemDto(
    string NotificationId,
    string? TaskId,
    string Kind,
    string Channel,
    string Status,
    string Title,
    string Message,
    DateTimeOffset CreatedAtUtc,
    DateTimeOffset? SentAtUtc);

public sealed record MailboxMessageListItemDto(
    string MessageId,
    string? TaskId,
    string Subject,
    string FromAddress,
    string Direction,
    string Status,
    string Summary,
    DateTimeOffset ReceivedAtUtc);

public sealed record CreateTaskRequest
{
    public string Title { get; init; } = string.Empty;
    public string Description { get; init; } = string.Empty;
    public string Source { get; init; } = "manual";
    public bool RequiresApproval { get; init; }
}

public sealed record ApprovalDecisionRequest
{
    public string Note { get; init; } = string.Empty;
}

public sealed record SimulateMailboxIngestRequest
{
    public string Subject { get; init; } = string.Empty;
    public string FromAddress { get; init; } = string.Empty;
    public string Body { get; init; } = string.Empty;
}

public sealed record WebhookInboundRequest
{
    public string Source { get; init; } = "webhook";
    public string EventName { get; init; } = string.Empty;
    public string Payload { get; init; } = string.Empty;
}

public sealed record ApiErrorResponse(string Msg, string Code);
