using TaskPilots.TaskAgents.Core;
using TaskPilots.TaskAgents.Repositories;
using TaskPilots.TaskAgents.WebApis.Realtime;

namespace TaskPilots.TaskAgents.WebApis.Services;

public sealed class RunApplicationService(
    ITaskRepository taskRepository,
    IEventRepository eventRepository,
    IRunRepository runRepository,
    ITaskAgentsRealtimeNotifier realtimeNotifier)
{
    public async Task<IReadOnlyList<AgentRunListItemDto>> GetRunsAsync(CancellationToken cancellationToken = default)
        => (await runRepository.ListAsync(cancellationToken)).Select(x => x.ToListItem()).ToList();

    public async Task<AgentRunDetailDto?> GetRunAsync(string runId, CancellationToken cancellationToken = default)
    {
        var run = await runRepository.GetAsync(runId, cancellationToken);
        if (run is null)
        {
            return null;
        }

        var checkpoints = await runRepository.ListCheckpointsAsync(runId, cancellationToken);
        var executionPath = await runRepository.ListExecutionPathAsync(runId, cancellationToken);
        return run.ToDetail(checkpoints, executionPath);
    }

    public async Task<AgentRunDetailDto?> RetryRunAsync(string runId, CancellationToken cancellationToken = default)
    {
        var existingRun = await runRepository.GetAsync(runId, cancellationToken);
        if (existingRun is null)
        {
            return null;
        }

        var task = await taskRepository.GetAsync(existingRun.TaskId, cancellationToken);
        if (task is null)
        {
            return null;
        }

        var nowUtc = DateTimeOffset.UtcNow;
        var retriedRun = new AgentRunRecord(
            $"run-{Guid.NewGuid():N}",
            task.TaskId,
            existingRun.Kind,
            RunStatus.Queued,
            "Queued after retry request.",
            nowUtc,
            null,
            null,
            "queued",
            null,
            "manual",
            null);
        var updatedTask = task with
        {
            Status = TaskStatus.WaitingForExecution,
            UpdatedAtUtc = nowUtc,
            NextExecutionAtUtc = nowUtc,
            LastRunId = retriedRun.RunId,
            LastError = null,
        };

        await runRepository.AddAsync(retriedRun, cancellationToken);
        await taskRepository.UpdateAsync(updatedTask, cancellationToken);
        await eventRepository.AddAsync(new TaskEventRecord(
            $"event-{Guid.NewGuid():N}",
            task.TaskId,
            "runRetried",
            "manual",
            $"Retry requested for run {runId}.",
            nowUtc), cancellationToken);
        await realtimeNotifier.BroadcastRefreshAsync("runsChanged", cancellationToken);
        return await GetRunAsync(retriedRun.RunId, cancellationToken);
    }
}
