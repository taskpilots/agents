using AgileLabs.Storage.PostgreSql;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace TaskPilots.TaskAgents.Repositories;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddTaskAgentsRepositories(this IServiceCollection services, IConfiguration configuration)
    {
        var mode = configuration["TaskAgents:Data:Mode"];
        if (string.Equals(mode, "Postgres", StringComparison.OrdinalIgnoreCase))
        {
            services.AddSingleton(new SqlConnectionManager(configuration));
            services.AddScoped<ITaskRepository, PostgresTaskRepository>();
            services.AddScoped<IEventRepository, PostgresEventRepository>();
            services.AddScoped<IRunRepository, PostgresRunRepository>();
            services.AddScoped<IApprovalRepository, PostgresApprovalRepository>();
            services.AddScoped<INotificationRepository, PostgresNotificationRepository>();
            services.AddScoped<IMailboxRepository, PostgresMailboxRepository>();
            return services;
        }

        services.AddSingleton<InMemoryTaskAgentsStore>();
        services.AddSingleton<ITaskRepository, InMemoryTaskRepository>();
        services.AddSingleton<IEventRepository, InMemoryEventRepository>();
        services.AddSingleton<IRunRepository, InMemoryRunRepository>();
        services.AddSingleton<IApprovalRepository, InMemoryApprovalRepository>();
        services.AddSingleton<INotificationRepository, InMemoryNotificationRepository>();
        services.AddSingleton<IMailboxRepository, InMemoryMailboxRepository>();
        return services;
    }
}
