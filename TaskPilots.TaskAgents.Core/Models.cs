namespace TaskPilots.TaskAgents.Core;

public sealed record TaskRecord(
    string TaskId,
    string Title,
    string Source,
    TaskStatus Status,
    string Summary,
    string Priority,
    bool RequiresApproval,
    DateTimeOffset CreatedAtUtc,
    DateTimeOffset UpdatedAtUtc,
    DateTimeOffset? NextExecutionAtUtc,
    string? LastRunId,
    string? LastError);

public sealed record TaskEventRecord(
    string EventId,
    string? TaskId,
    string EventType,
    string Source,
    string PayloadSummary,
    DateTimeOffset CreatedAtUtc);

public sealed record AgentRunRecord(
    string RunId,
    string TaskId,
    string Kind,
    RunStatus Status,
    string Summary,
    DateTimeOffset RequestedAtUtc,
    DateTimeOffset? StartedAtUtc,
    DateTimeOffset? CompletedAtUtc,
    string? CurrentStep,
    string? OutputSummary,
    string RequestedBy,
    string? WaitingReason);

public sealed record RunCheckpointRecord(
    string CheckpointId,
    string RunId,
    string StepName,
    string Snapshot,
    DateTimeOffset CreatedAtUtc);

public sealed record RunExecutionPathEntryRecord(
    string EntryId,
    string RunId,
    string StepName,
    string Detail,
    DateTimeOffset CreatedAtUtc);

public sealed record ApprovalRecord(
    string ApprovalId,
    string TaskId,
    string? RunId,
    ApprovalStatus Status,
    string Title,
    string RequestedBy,
    string RiskLevel,
    string? DecisionNote,
    DateTimeOffset RequestedAtUtc,
    DateTimeOffset? ResolvedAtUtc);

public sealed record NotificationRecord(
    string NotificationId,
    string? TaskId,
    string Kind,
    string Channel,
    NotificationStatus Status,
    string Title,
    string Message,
    DateTimeOffset CreatedAtUtc,
    DateTimeOffset? SentAtUtc);

public sealed record MailboxMessageRecord(
    string MessageId,
    string? TaskId,
    string Subject,
    string FromAddress,
    string Direction,
    MailboxMessageStatus Status,
    string Summary,
    DateTimeOffset ReceivedAtUtc);
