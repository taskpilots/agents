using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging.Abstractions;
using TaskPilots.TaskAgents.Core;
using TaskPilots.TaskAgents.Repositories;
using TaskPilots.TaskAgents.WebApis.Services;
using TaskPilots.TaskAgents.WebApis.Workers;

namespace TaskPilots.TaskAgents.WebApis.Tests;

public sealed class RunOrchestratorWorkerTests
{
    [Fact]
    public async Task RunOnceAsync_WhenRunDoesNotRequireApproval_ShouldCompleteTask()
    {
        var store = new InMemoryTaskAgentsStore();
        store.WithLock(() =>
        {
            store.Tasks.Clear();
            store.Events.Clear();
            store.Runs.Clear();
            store.Approvals.Clear();
            store.Notifications.Clear();
        });

        await using var provider = TestServiceCollectionFactory.CreateProvider(store);
        var provisioningService = provider.GetRequiredService<TaskProvisioningService>();
        var task = await provisioningService.CreateTaskAsync("Complete task", "test", "manual", false);
        var worker = new RunOrchestratorWorker(
            NullLogger<RunOrchestratorWorker>.Instance,
            provider.GetRequiredService<IServiceScopeFactory>());

        await worker.RunOnceAsync();

        var updatedTask = await provider.GetRequiredService<ITaskRepository>().GetAsync(task.TaskId);
        Assert.NotNull(updatedTask);
        Assert.Equal(TaskStatus.Completed, updatedTask.Status);
    }

    [Fact]
    public async Task RunOnceAsync_WhenRunRequiresApproval_ShouldCreateApproval()
    {
        var store = new InMemoryTaskAgentsStore();
        store.WithLock(() =>
        {
            store.Tasks.Clear();
            store.Events.Clear();
            store.Runs.Clear();
            store.Approvals.Clear();
            store.Notifications.Clear();
        });

        await using var provider = TestServiceCollectionFactory.CreateProvider(store);
        var provisioningService = provider.GetRequiredService<TaskProvisioningService>();
        var task = await provisioningService.CreateTaskAsync("Approval task", "test", "manual", true);
        var worker = new RunOrchestratorWorker(
            NullLogger<RunOrchestratorWorker>.Instance,
            provider.GetRequiredService<IServiceScopeFactory>());

        await worker.RunOnceAsync();

        var updatedTask = await provider.GetRequiredService<ITaskRepository>().GetAsync(task.TaskId);
        var approvals = await provider.GetRequiredService<IApprovalRepository>().ListAsync(task.TaskId);
        Assert.NotNull(updatedTask);
        Assert.Equal(TaskStatus.WaitingForApproval, updatedTask.Status);
        Assert.NotEmpty(approvals);
    }
}
