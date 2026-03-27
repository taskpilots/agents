using TaskPilots.TaskAgents.Core;
using TaskPilots.TaskAgents.Repositories;
using TaskPilots.TaskAgents.WebApis.Realtime;

namespace TaskPilots.TaskAgents.WebApis.Services;

public sealed class TaskProvisioningService(
    ITaskRepository taskRepository,
    IEventRepository eventRepository,
    IRunRepository runRepository,
    ITaskAgentsRealtimeNotifier realtimeNotifier)
{
    public async Task<TaskRecord> CreateTaskAsync(
        string title,
        string summary,
        string source,
        bool requiresApproval,
        string priority = "medium",
        CancellationToken cancellationToken = default)
    {
        var nowUtc = DateTimeOffset.UtcNow;
        var taskId = $"task-{Guid.NewGuid():N}";
        var runId = $"run-{Guid.NewGuid():N}";

        var task = new TaskRecord(
            taskId,
            title,
            source,
            TaskStatus.WaitingForExecution,
            summary,
            priority,
            requiresApproval,
            nowUtc,
            nowUtc,
            nowUtc,
            runId,
            null);
        var run = new AgentRunRecord(
            runId,
            taskId,
            "main-agent",
            RunStatus.Queued,
            "Queued for orchestration.",
            nowUtc,
            null,
            null,
            "queued",
            null,
            source,
            null);
        var taskEvent = new TaskEventRecord(
            $"event-{Guid.NewGuid():N}",
            taskId,
            "taskCreated",
            source,
            summary,
            nowUtc);

        await taskRepository.AddAsync(task, cancellationToken);
        await runRepository.AddAsync(run, cancellationToken);
        await eventRepository.AddAsync(taskEvent, cancellationToken);
        await realtimeNotifier.BroadcastRefreshAsync("tasksChanged", cancellationToken);
        return task;
    }
}
