using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging.Abstractions;
using TaskPilots.TaskAgents.Core;
using TaskPilots.TaskAgents.Repositories;
using TaskPilots.TaskAgents.WebApis.Integrations;
using TaskPilots.TaskAgents.WebApis.Realtime;
using TaskPilots.TaskAgents.WebApis.Services;
using TaskPilots.TaskAgents.WebApis.Workers;

namespace TaskPilots.TaskAgents.WebApis.Tests;

internal static class TestServiceCollectionFactory
{
    public static ServiceProvider CreateProvider(InMemoryTaskAgentsStore? store = null)
    {
        var services = new ServiceCollection();
        services.AddSingleton(store ?? new InMemoryTaskAgentsStore());
        services.AddSingleton<ITaskRepository, InMemoryTaskRepository>();
        services.AddSingleton<IEventRepository, InMemoryEventRepository>();
        services.AddSingleton<IRunRepository, InMemoryRunRepository>();
        services.AddSingleton<IApprovalRepository, InMemoryApprovalRepository>();
        services.AddSingleton<INotificationRepository, InMemoryNotificationRepository>();
        services.AddSingleton<IMailboxRepository, InMemoryMailboxRepository>();
        services.AddSingleton<IOpenAiClient, FakeOpenAiClient>();
        services.AddSingleton<IMailSender, FakeMailSender>();
        services.AddSingleton<IMailboxPoller, FakeMailboxPoller>();
        services.AddSingleton<IWebhookIngressHandler, FakeWebhookIngressHandler>();
        services.AddSingleton<ITaskAgentsRealtimeNotifier, NoopRealtimeNotifier>();
        services.AddScoped<TaskProvisioningService>();
        services.AddScoped<SystemApplicationService>();
        services.AddScoped<TaskApplicationService>();
        services.AddScoped<EventApplicationService>();
        services.AddScoped<RunApplicationService>();
        services.AddScoped<ApprovalApplicationService>();
        services.AddScoped<NotificationApplicationService>();
        services.AddScoped<MailboxApplicationService>();
        services.AddScoped<WebhookApplicationService>();
        services.AddSingleton<SchedulerWorker>();
        services.AddSingleton<RunOrchestratorWorker>(_ => new RunOrchestratorWorker(
            NullLogger<RunOrchestratorWorker>.Instance,
            _.GetRequiredService<IServiceScopeFactory>()));

        return services.BuildServiceProvider();
    }
}

internal sealed class NoopRealtimeNotifier : ITaskAgentsRealtimeNotifier
{
    public List<string> Messages { get; } = [];

    public Task BroadcastRefreshAsync(string message, CancellationToken cancellationToken = default)
    {
        Messages.Add(message);
        return Task.CompletedTask;
    }
}
