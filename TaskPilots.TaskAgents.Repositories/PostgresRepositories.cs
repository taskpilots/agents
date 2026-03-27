using AgileLabs.Storage.PostgreSql;
using TaskPilots.TaskAgents.Core;

namespace TaskPilots.TaskAgents.Repositories;

public abstract class TaskAgentsPostgresRepositoryBase(SqlConnectionManager connectionManager)
{
    protected SqlConnectionManager ConnectionManager { get; } = connectionManager;

    protected static Exception NotImplemented()
        => new NotSupportedException("PostgreSQL repositories are scaffolded but not implemented in this bootstrap.");
}

public sealed class PostgresTaskRepository(SqlConnectionManager connectionManager) : TaskAgentsPostgresRepositoryBase(connectionManager), ITaskRepository
{
    public Task<IReadOnlyList<TaskRecord>> ListAsync(CancellationToken cancellationToken = default) => throw NotImplemented();
    public Task<TaskRecord?> GetAsync(string taskId, CancellationToken cancellationToken = default) => throw NotImplemented();
    public Task AddAsync(TaskRecord task, CancellationToken cancellationToken = default) => throw NotImplemented();
    public Task UpdateAsync(TaskRecord task, CancellationToken cancellationToken = default) => throw NotImplemented();
}

public sealed class PostgresEventRepository(SqlConnectionManager connectionManager) : TaskAgentsPostgresRepositoryBase(connectionManager), IEventRepository
{
    public Task<IReadOnlyList<TaskEventRecord>> ListAsync(string? taskId = null, CancellationToken cancellationToken = default) => throw NotImplemented();
    public Task AddAsync(TaskEventRecord taskEvent, CancellationToken cancellationToken = default) => throw NotImplemented();
}

public sealed class PostgresRunRepository(SqlConnectionManager connectionManager) : TaskAgentsPostgresRepositoryBase(connectionManager), IRunRepository
{
    public Task<IReadOnlyList<AgentRunRecord>> ListAsync(CancellationToken cancellationToken = default) => throw NotImplemented();
    public Task<IReadOnlyList<AgentRunRecord>> ListByTaskIdAsync(string taskId, CancellationToken cancellationToken = default) => throw NotImplemented();
    public Task<AgentRunRecord?> GetAsync(string runId, CancellationToken cancellationToken = default) => throw NotImplemented();
    public Task AddAsync(AgentRunRecord run, CancellationToken cancellationToken = default) => throw NotImplemented();
    public Task UpdateAsync(AgentRunRecord run, CancellationToken cancellationToken = default) => throw NotImplemented();
    public Task<IReadOnlyList<RunCheckpointRecord>> ListCheckpointsAsync(string runId, CancellationToken cancellationToken = default) => throw NotImplemented();
    public Task<IReadOnlyList<RunExecutionPathEntryRecord>> ListExecutionPathAsync(string runId, CancellationToken cancellationToken = default) => throw NotImplemented();
    public Task AddCheckpointAsync(RunCheckpointRecord checkpoint, CancellationToken cancellationToken = default) => throw NotImplemented();
    public Task AddExecutionPathEntryAsync(RunExecutionPathEntryRecord entry, CancellationToken cancellationToken = default) => throw NotImplemented();
}

public sealed class PostgresApprovalRepository(SqlConnectionManager connectionManager) : TaskAgentsPostgresRepositoryBase(connectionManager), IApprovalRepository
{
    public Task<IReadOnlyList<ApprovalRecord>> ListAsync(string? taskId = null, CancellationToken cancellationToken = default) => throw NotImplemented();
    public Task<ApprovalRecord?> GetAsync(string approvalId, CancellationToken cancellationToken = default) => throw NotImplemented();
    public Task AddAsync(ApprovalRecord approval, CancellationToken cancellationToken = default) => throw NotImplemented();
    public Task UpdateAsync(ApprovalRecord approval, CancellationToken cancellationToken = default) => throw NotImplemented();
}

public sealed class PostgresNotificationRepository(SqlConnectionManager connectionManager) : TaskAgentsPostgresRepositoryBase(connectionManager), INotificationRepository
{
    public Task<IReadOnlyList<NotificationRecord>> ListAsync(string? taskId = null, CancellationToken cancellationToken = default) => throw NotImplemented();
    public Task AddAsync(NotificationRecord notification, CancellationToken cancellationToken = default) => throw NotImplemented();
    public Task UpdateAsync(NotificationRecord notification, CancellationToken cancellationToken = default) => throw NotImplemented();
}

public sealed class PostgresMailboxRepository(SqlConnectionManager connectionManager) : TaskAgentsPostgresRepositoryBase(connectionManager), IMailboxRepository
{
    public Task<IReadOnlyList<MailboxMessageRecord>> ListAsync(CancellationToken cancellationToken = default) => throw NotImplemented();
    public Task AddAsync(MailboxMessageRecord message, CancellationToken cancellationToken = default) => throw NotImplemented();
}
