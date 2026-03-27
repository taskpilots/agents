using Microsoft.Extensions.Hosting;
using TaskPilots.TaskAgents.Repositories;
using TaskPilots.TaskAgents.WebApis.Integrations;
using TaskPilots.TaskAgents.WebApis.Services;

namespace TaskPilots.TaskAgents.WebApis.Workers;

public sealed class SchedulerWorker(
    ILogger<SchedulerWorker> logger,
    IServiceScopeFactory scopeFactory,
    IMailboxPoller mailboxPoller) : BackgroundService
{
    public async Task RunOnceAsync(CancellationToken cancellationToken = default)
    {
        _ = await mailboxPoller.PollAsync(cancellationToken);

        using var scope = scopeFactory.CreateScope();
        var taskRepository = scope.ServiceProvider.GetRequiredService<ITaskRepository>();
        var provisioningService = scope.ServiceProvider.GetRequiredService<TaskProvisioningService>();
        var tasks = await taskRepository.ListAsync(cancellationToken);
        if (tasks.Count == 0)
        {
            logger.LogInformation("Scheduler worker creating a bootstrap task.");
            await provisioningService.CreateTaskAsync(
                "Scheduled intelligence refresh",
                "Synthetic task created by scheduler bootstrap.",
                "scheduler",
                requiresApproval: false,
                priority: "low",
                cancellationToken: cancellationToken);
        }
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        using var timer = new PeriodicTimer(TimeSpan.FromSeconds(30));
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
                logger.LogError(exception, "Scheduler worker iteration failed.");
            }

            await timer.WaitForNextTickAsync(stoppingToken);
        }
    }
}
