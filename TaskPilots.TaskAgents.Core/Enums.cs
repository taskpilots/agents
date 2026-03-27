namespace TaskPilots.TaskAgents.Core;

public enum TaskStatus
{
    Created,
    Triaged,
    Planned,
    WaitingForExecution,
    Running,
    WaitingForApproval,
    WaitingForExternalInput,
    PartiallyCompleted,
    Completed,
    Archived,
    Failed,
}

public enum RunStatus
{
    Queued,
    Running,
    WaitingForApproval,
    Completed,
    Failed,
    Cancelled,
}

public enum ApprovalStatus
{
    Pending,
    Approved,
    Rejected,
    Expired,
}

public enum NotificationStatus
{
    Pending,
    Sent,
    Failed,
}

public enum MailboxMessageStatus
{
    Received,
    LinkedToTask,
    Processed,
    Archived,
}
