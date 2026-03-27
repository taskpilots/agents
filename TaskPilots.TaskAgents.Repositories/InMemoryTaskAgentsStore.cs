using TaskPilots.TaskAgents.Core;

namespace TaskPilots.TaskAgents.Repositories;

public sealed class InMemoryTaskAgentsStore
{
    private readonly object _syncRoot = new();

    public InMemoryTaskAgentsStore()
    {
        Seed();
    }

    public List<TaskRecord> Tasks { get; } = [];
    public List<TaskEventRecord> Events { get; } = [];
    public List<AgentRunRecord> Runs { get; } = [];
    public List<RunCheckpointRecord> Checkpoints { get; } = [];
    public List<RunExecutionPathEntryRecord> ExecutionPathEntries { get; } = [];
    public List<ApprovalRecord> Approvals { get; } = [];
    public List<NotificationRecord> Notifications { get; } = [];
    public List<MailboxMessageRecord> MailboxMessages { get; } = [];

    public T WithLock<T>(Func<T> work)
    {
        lock (_syncRoot)
        {
            return work();
        }
    }

    public void WithLock(Action work)
    {
        lock (_syncRoot)
        {
            work();
        }
    }

    private void Seed()
    {
        var nowUtc = DateTimeOffset.UtcNow;
        var task = new TaskRecord(
            "task-seed-001",
            "Review quarterly AI vendor landscape",
            "scheduler",
            TaskStatus.WaitingForApproval,
            "Track top vendor moves and produce an approval-gated summary.",
            "high",
            true,
            nowUtc.AddHours(-3),
            nowUtc.AddMinutes(-15),
            nowUtc.AddMinutes(15),
            "run-seed-001",
            null);
        Tasks.Add(task);

        Events.Add(new TaskEventRecord(
            "event-seed-001",
            task.TaskId,
            "taskCreated",
            "scheduler",
            "Seed task created to populate the dashboard.",
            nowUtc.AddHours(-3)));
        Events.Add(new TaskEventRecord(
            "event-seed-002",
            task.TaskId,
            "runPausedForApproval",
            "system",
            "Main agent requested approval before external delivery.",
            nowUtc.AddMinutes(-20)));

        Runs.Add(new AgentRunRecord(
            "run-seed-001",
            task.TaskId,
            "main-agent",
            RunStatus.WaitingForApproval,
            "Prepared a structured summary and is waiting for approval.",
            nowUtc.AddHours(-2),
            nowUtc.AddHours(-2).AddMinutes(1),
            null,
            "approval_gate",
            "A draft report is ready for review.",
            "scheduler",
            "Awaiting approval before sending the report."));

        Checkpoints.Add(new RunCheckpointRecord(
            "checkpoint-seed-001",
            "run-seed-001",
            "planning_complete",
            "{\"phase\":\"planning\",\"result\":\"three work items generated\"}",
            nowUtc.AddHours(-2).AddMinutes(10)));

        ExecutionPathEntries.Add(new RunExecutionPathEntryRecord(
            "path-seed-001",
            "run-seed-001",
            "search",
            "Collected six candidate sources and filtered three primary sources.",
            nowUtc.AddHours(-2).AddMinutes(5)));
        ExecutionPathEntries.Add(new RunExecutionPathEntryRecord(
            "path-seed-002",
            "run-seed-001",
            "draft",
            "Built draft report and marked it for approval.",
            nowUtc.AddMinutes(-20)));

        Approvals.Add(new ApprovalRecord(
            "approval-seed-001",
            task.TaskId,
            "run-seed-001",
            ApprovalStatus.Pending,
            "Approve external delivery of vendor landscape summary",
            "main-agent",
            "medium",
            null,
            nowUtc.AddMinutes(-20),
            null));

        Notifications.Add(new NotificationRecord(
            "notification-seed-001",
            task.TaskId,
            "approvalRequested",
            "in_app",
            NotificationStatus.Sent,
            "Approval needed",
            "A draft summary is waiting for your decision.",
            nowUtc.AddMinutes(-19),
            nowUtc.AddMinutes(-19)));

        MailboxMessages.Add(new MailboxMessageRecord(
            "mail-seed-001",
            task.TaskId,
            "Analyst request: follow competitor filings",
            "ops@example.local",
            "inbound",
            MailboxMessageStatus.LinkedToTask,
            "Original inbound request that led to the seeded task.",
            nowUtc.AddHours(-3).AddMinutes(-10)));
    }
}
