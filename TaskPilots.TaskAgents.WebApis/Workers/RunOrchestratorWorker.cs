using Microsoft.Extensions.Hosting;
using TaskPilots.TaskAgents.Core;
using TaskPilots.TaskAgents.Repositories;
using TaskPilots.TaskAgents.WebApis.Integrations;
using TaskPilots.TaskAgents.WebApis.Realtime;

namespace TaskPilots.TaskAgents.WebApis.Workers;

public sealed class RunOrchestratorWorker(
    ILogger<RunOrchestratorWorker> logger,
    IServiceScopeFactory scopeFactory) : BackgroundService
{
    public async Task RunOnceAsync(CancellationToken cancellationToken = default)
    {
        using var scope = scopeFactory.CreateScope();
        var taskRepository = scope.ServiceProvider.GetRequiredService<ITaskRepository>();
        var eventRepository = scope.ServiceProvider.GetRequiredService<IEventRepository>();
        var runRepository = scope.ServiceProvider.GetRequiredService<IRunRepository>();
        var approvalRepository = scope.ServiceProvider.GetRequiredService<IApprovalRepository>();
        var notificationRepository = scope.ServiceProvider.GetRequiredService<INotificationRepository>();
        var openAiClient = scope.ServiceProvider.GetRequiredService<IOpenAiClient>();
        var realtimeNotifier = scope.ServiceProvider.GetRequiredService<ITaskAgentsRealtimeNotifier>();

        var run = (await runRepository.ListAsync(cancellationToken)).FirstOrDefault(x => x.Status == RunStatus.Queued);
        if (run is null)
        {
            return;
        }

        var task = await taskRepository.GetAsync(run.TaskId, cancellationToken);
        if (task is null)
        {
            return;
        }

        var nowUtc = DateTimeOffset.UtcNow;
        var summary = await openAiClient.GenerateRunSummaryAsync(task, cancellationToken);
        await runRepository.UpdateAsync(run with
        {
            Status = RunStatus.Running,
            StartedAtUtc = nowUtc,
            CurrentStep = "analysis",
            Summary = "Run started by orchestrator.",
        }, cancellationToken);
        await taskRepository.UpdateAsync(task with
        {
            Status = TaskStatus.Running,
            UpdatedAtUtc = nowUtc,
            NextExecutionAtUtc = null,
        }, cancellationToken);
        await runRepository.AddExecutionPathEntryAsync(new RunExecutionPathEntryRecord(
            $"path-{Guid.NewGuid():N}",
            run.RunId,
            "analysis_started",
            "Run picked up by orchestrator.",
            nowUtc), cancellationToken);

        if (task.RequiresApproval)
        {
            var approval = new ApprovalRecord(
                $"approval-{Guid.NewGuid():N}",
                task.TaskId,
                run.RunId,
                ApprovalStatus.Pending,
                $"Approve delivery for {task.Title}",
                "main-agent",
                "medium",
                null,
                nowUtc,
                null);
            await approvalRepository.AddAsync(approval, cancellationToken);
            await runRepository.UpdateAsync(run with
            {
                Status = RunStatus.WaitingForApproval,
                StartedAtUtc = nowUtc,
                CurrentStep = "approval_gate",
                OutputSummary = summary,
                WaitingReason = "Awaiting approval before external action.",
                Summary = "Run paused for approval.",
            }, cancellationToken);
            await taskRepository.UpdateAsync(task with
            {
                Status = TaskStatus.WaitingForApproval,
                UpdatedAtUtc = nowUtc,
            }, cancellationToken);
            await eventRepository.AddAsync(new TaskEventRecord(
                $"event-{Guid.NewGuid():N}",
                task.TaskId,
                "approvalRequested",
                "orchestrator",
                "Run entered approval gate.",
                nowUtc), cancellationToken);
            await notificationRepository.AddAsync(new NotificationRecord(
                $"notification-{Guid.NewGuid():N}",
                task.TaskId,
                "approvalRequested",
                "in_app",
                NotificationStatus.Sent,
                "Approval requested",
                $"Run {run.RunId} is waiting for approval.",
                nowUtc,
                nowUtc), cancellationToken);
        }
        else
        {
            await runRepository.UpdateAsync(run with
            {
                Status = RunStatus.Completed,
                StartedAtUtc = nowUtc,
                CompletedAtUtc = nowUtc,
                CurrentStep = "completed",
                OutputSummary = summary,
                WaitingReason = null,
                Summary = "Run completed.",
            }, cancellationToken);
            await taskRepository.UpdateAsync(task with
            {
                Status = TaskStatus.Completed,
                UpdatedAtUtc = nowUtc,
            }, cancellationToken);
            await eventRepository.AddAsync(new TaskEventRecord(
                $"event-{Guid.NewGuid():N}",
                task.TaskId,
                "runCompleted",
                "orchestrator",
                "Run completed successfully.",
                nowUtc), cancellationToken);
            await notificationRepository.AddAsync(new NotificationRecord(
                $"notification-{Guid.NewGuid():N}",
                task.TaskId,
                "runCompleted",
                "in_app",
                NotificationStatus.Sent,
                "Task completed",
                $"Task {task.Title} completed.",
                nowUtc,
                nowUtc), cancellationToken);
        }

        await runRepository.AddCheckpointAsync(new RunCheckpointRecord(
            $"checkpoint-{Guid.NewGuid():N}",
            run.RunId,
            "orchestration_cycle",
            "{\"result\":\"cycle completed\"}",
            nowUtc), cancellationToken);
        await realtimeNotifier.BroadcastRefreshAsync("runsChanged", cancellationToken);
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        using var timer = new PeriodicTimer(TimeSpan.FromSeconds(10));
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                await RunOnceAsync(stoppingToken);
            }
            catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested)
            {
                return;
            }
            catch (Exception exception)
            {
                logger.LogError(exception, "Run orchestrator iteration failed.");
            }

            await timer.WaitForNextTickAsync(stoppingToken);
        }
    }
}
