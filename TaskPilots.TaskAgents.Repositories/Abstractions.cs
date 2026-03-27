using TaskPilots.TaskAgents.Core;

namespace TaskPilots.TaskAgents.Repositories;

public interface ITaskRepository
{
    Task<IReadOnlyList<TaskRecord>> ListAsync(CancellationToken cancellationToken = default);
    Task<TaskRecord?> GetAsync(string taskId, CancellationToken cancellationToken = default);
    Task AddAsync(TaskRecord task, CancellationToken cancellationToken = default);
    Task UpdateAsync(TaskRecord task, CancellationToken cancellationToken = default);
}

public interface IEventRepository
{
    Task<IReadOnlyList<TaskEventRecord>> ListAsync(string? taskId = null, CancellationToken cancellationToken = default);
    Task AddAsync(TaskEventRecord taskEvent, CancellationToken cancellationToken = default);
}

public interface IRunRepository
{
    Task<IReadOnlyList<AgentRunRecord>> ListAsync(CancellationToken cancellationToken = default);
    Task<IReadOnlyList<AgentRunRecord>> ListByTaskIdAsync(string taskId, CancellationToken cancellationToken = default);
    Task<AgentRunRecord?> GetAsync(string runId, CancellationToken cancellationToken = default);
    Task AddAsync(AgentRunRecord run, CancellationToken cancellationToken = default);
    Task UpdateAsync(AgentRunRecord run, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<RunCheckpointRecord>> ListCheckpointsAsync(string runId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<RunExecutionPathEntryRecord>> ListExecutionPathAsync(string runId, CancellationToken cancellationToken = default);
    Task AddCheckpointAsync(RunCheckpointRecord checkpoint, CancellationToken cancellationToken = default);
    Task AddExecutionPathEntryAsync(RunExecutionPathEntryRecord entry, CancellationToken cancellationToken = default);
}

public interface IApprovalRepository
{
    Task<IReadOnlyList<ApprovalRecord>> ListAsync(string? taskId = null, CancellationToken cancellationToken = default);
    Task<ApprovalRecord?> GetAsync(string approvalId, CancellationToken cancellationToken = default);
    Task AddAsync(ApprovalRecord approval, CancellationToken cancellationToken = default);
    Task UpdateAsync(ApprovalRecord approval, CancellationToken cancellationToken = default);
}

public interface INotificationRepository
{
    Task<IReadOnlyList<NotificationRecord>> ListAsync(string? taskId = null, CancellationToken cancellationToken = default);
    Task AddAsync(NotificationRecord notification, CancellationToken cancellationToken = default);
    Task UpdateAsync(NotificationRecord notification, CancellationToken cancellationToken = default);
}

public interface IMailboxRepository
{
    Task<IReadOnlyList<MailboxMessageRecord>> ListAsync(CancellationToken cancellationToken = default);
    Task AddAsync(MailboxMessageRecord message, CancellationToken cancellationToken = default);
}
