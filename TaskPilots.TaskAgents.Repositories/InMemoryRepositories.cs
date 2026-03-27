using TaskPilots.TaskAgents.Core;

namespace TaskPilots.TaskAgents.Repositories;

public sealed class InMemoryTaskRepository(InMemoryTaskAgentsStore store) : ITaskRepository
{
    public Task<IReadOnlyList<TaskRecord>> ListAsync(CancellationToken cancellationToken = default)
        => Task.FromResult<IReadOnlyList<TaskRecord>>(store.WithLock(() => store.Tasks.OrderByDescending(x => x.UpdatedAtUtc).ToList()));

    public Task<TaskRecord?> GetAsync(string taskId, CancellationToken cancellationToken = default)
        => Task.FromResult(store.WithLock(() => store.Tasks.SingleOrDefault(x => x.TaskId == taskId)));

    public Task AddAsync(TaskRecord task, CancellationToken cancellationToken = default)
    {
        store.WithLock(() => store.Tasks.Add(task));
        return Task.CompletedTask;
    }

    public Task UpdateAsync(TaskRecord task, CancellationToken cancellationToken = default)
    {
        store.WithLock(() =>
        {
            var index = store.Tasks.FindIndex(x => x.TaskId == task.TaskId);
            if (index >= 0)
            {
                store.Tasks[index] = task;
            }
        });
        return Task.CompletedTask;
    }
}

public sealed class InMemoryEventRepository(InMemoryTaskAgentsStore store) : IEventRepository
{
    public Task<IReadOnlyList<TaskEventRecord>> ListAsync(string? taskId = null, CancellationToken cancellationToken = default)
        => Task.FromResult<IReadOnlyList<TaskEventRecord>>(store.WithLock(() =>
            store.Events.Where(x => taskId is null || x.TaskId == taskId).OrderByDescending(x => x.CreatedAtUtc).ToList()));

    public Task AddAsync(TaskEventRecord taskEvent, CancellationToken cancellationToken = default)
    {
        store.WithLock(() => store.Events.Add(taskEvent));
        return Task.CompletedTask;
    }
}

public sealed class InMemoryRunRepository(InMemoryTaskAgentsStore store) : IRunRepository
{
    public Task<IReadOnlyList<AgentRunRecord>> ListAsync(CancellationToken cancellationToken = default)
        => Task.FromResult<IReadOnlyList<AgentRunRecord>>(store.WithLock(() => store.Runs.OrderByDescending(x => x.RequestedAtUtc).ToList()));

    public Task<IReadOnlyList<AgentRunRecord>> ListByTaskIdAsync(string taskId, CancellationToken cancellationToken = default)
        => Task.FromResult<IReadOnlyList<AgentRunRecord>>(store.WithLock(() =>
            store.Runs.Where(x => x.TaskId == taskId).OrderByDescending(x => x.RequestedAtUtc).ToList()));

    public Task<AgentRunRecord?> GetAsync(string runId, CancellationToken cancellationToken = default)
        => Task.FromResult(store.WithLock(() => store.Runs.SingleOrDefault(x => x.RunId == runId)));

    public Task AddAsync(AgentRunRecord run, CancellationToken cancellationToken = default)
    {
        store.WithLock(() => store.Runs.Add(run));
        return Task.CompletedTask;
    }

    public Task UpdateAsync(AgentRunRecord run, CancellationToken cancellationToken = default)
    {
        store.WithLock(() =>
        {
            var index = store.Runs.FindIndex(x => x.RunId == run.RunId);
            if (index >= 0)
            {
                store.Runs[index] = run;
            }
        });
        return Task.CompletedTask;
    }

    public Task<IReadOnlyList<RunCheckpointRecord>> ListCheckpointsAsync(string runId, CancellationToken cancellationToken = default)
        => Task.FromResult<IReadOnlyList<RunCheckpointRecord>>(store.WithLock(() =>
            store.Checkpoints.Where(x => x.RunId == runId).OrderByDescending(x => x.CreatedAtUtc).ToList()));

    public Task<IReadOnlyList<RunExecutionPathEntryRecord>> ListExecutionPathAsync(string runId, CancellationToken cancellationToken = default)
        => Task.FromResult<IReadOnlyList<RunExecutionPathEntryRecord>>(store.WithLock(() =>
            store.ExecutionPathEntries.Where(x => x.RunId == runId).OrderBy(x => x.CreatedAtUtc).ToList()));

    public Task AddCheckpointAsync(RunCheckpointRecord checkpoint, CancellationToken cancellationToken = default)
    {
        store.WithLock(() => store.Checkpoints.Add(checkpoint));
        return Task.CompletedTask;
    }

    public Task AddExecutionPathEntryAsync(RunExecutionPathEntryRecord entry, CancellationToken cancellationToken = default)
    {
        store.WithLock(() => store.ExecutionPathEntries.Add(entry));
        return Task.CompletedTask;
    }
}

public sealed class InMemoryApprovalRepository(InMemoryTaskAgentsStore store) : IApprovalRepository
{
    public Task<IReadOnlyList<ApprovalRecord>> ListAsync(string? taskId = null, CancellationToken cancellationToken = default)
        => Task.FromResult<IReadOnlyList<ApprovalRecord>>(store.WithLock(() =>
            store.Approvals.Where(x => taskId is null || x.TaskId == taskId).OrderByDescending(x => x.RequestedAtUtc).ToList()));

    public Task<ApprovalRecord?> GetAsync(string approvalId, CancellationToken cancellationToken = default)
        => Task.FromResult(store.WithLock(() => store.Approvals.SingleOrDefault(x => x.ApprovalId == approvalId)));

    public Task AddAsync(ApprovalRecord approval, CancellationToken cancellationToken = default)
    {
        store.WithLock(() => store.Approvals.Add(approval));
        return Task.CompletedTask;
    }

    public Task UpdateAsync(ApprovalRecord approval, CancellationToken cancellationToken = default)
    {
        store.WithLock(() =>
        {
            var index = store.Approvals.FindIndex(x => x.ApprovalId == approval.ApprovalId);
            if (index >= 0)
            {
                store.Approvals[index] = approval;
            }
        });
        return Task.CompletedTask;
    }
}

public sealed class InMemoryNotificationRepository(InMemoryTaskAgentsStore store) : INotificationRepository
{
    public Task<IReadOnlyList<NotificationRecord>> ListAsync(string? taskId = null, CancellationToken cancellationToken = default)
        => Task.FromResult<IReadOnlyList<NotificationRecord>>(store.WithLock(() =>
            store.Notifications.Where(x => taskId is null || x.TaskId == taskId).OrderByDescending(x => x.CreatedAtUtc).ToList()));

    public Task AddAsync(NotificationRecord notification, CancellationToken cancellationToken = default)
    {
        store.WithLock(() => store.Notifications.Add(notification));
        return Task.CompletedTask;
    }

    public Task UpdateAsync(NotificationRecord notification, CancellationToken cancellationToken = default)
    {
        store.WithLock(() =>
        {
            var index = store.Notifications.FindIndex(x => x.NotificationId == notification.NotificationId);
            if (index >= 0)
            {
                store.Notifications[index] = notification;
            }
        });
        return Task.CompletedTask;
    }
}

public sealed class InMemoryMailboxRepository(InMemoryTaskAgentsStore store) : IMailboxRepository
{
    public Task<IReadOnlyList<MailboxMessageRecord>> ListAsync(CancellationToken cancellationToken = default)
        => Task.FromResult<IReadOnlyList<MailboxMessageRecord>>(store.WithLock(() => store.MailboxMessages.OrderByDescending(x => x.ReceivedAtUtc).ToList()));

    public Task AddAsync(MailboxMessageRecord message, CancellationToken cancellationToken = default)
    {
        store.WithLock(() => store.MailboxMessages.Add(message));
        return Task.CompletedTask;
    }
}
