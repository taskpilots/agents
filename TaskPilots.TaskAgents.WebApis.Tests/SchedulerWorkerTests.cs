using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging.Abstractions;
using TaskPilots.TaskAgents.Repositories;
using TaskPilots.TaskAgents.WebApis.Integrations;
using TaskPilots.TaskAgents.WebApis.Workers;

namespace TaskPilots.TaskAgents.WebApis.Tests;

public sealed class SchedulerWorkerTests
{
    [Fact]
    public async Task RunOnceAsync_WhenStoreEmpty_ShouldCreateBootstrapTask()
    {
        var store = new InMemoryTaskAgentsStore();
        store.WithLock(() =>
        {
            store.Tasks.Clear();
            store.Events.Clear();
            store.Runs.Clear();
            store.Approvals.Clear();
            store.Notifications.Clear();
            store.MailboxMessages.Clear();
        });

        await using var provider = TestServiceCollectionFactory.CreateProvider(store);
        var worker = new SchedulerWorker(
            NullLogger<SchedulerWorker>.Instance,
            provider.GetRequiredService<IServiceScopeFactory>(),
            provider.GetRequiredService<IMailboxPoller>());

        await worker.RunOnceAsync();

        Assert.NotEmpty(await provider.GetRequiredService<ITaskRepository>().ListAsync());
    }
}
