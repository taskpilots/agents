using AgileLabs;
using AgileLabs.AppRegisters;
using AgileLabs.Json;
using AgileLabs.WebApp.Hosting;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http.Connections;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;
using TaskPilots.TaskAgents.Repositories;
using TaskPilots.TaskAgents.WebApis.Integrations;
using TaskPilots.TaskAgents.WebApis.Realtime;
using TaskPilots.TaskAgents.WebApis.Services;
using TaskPilots.TaskAgents.WebApis.Workers;

namespace TaskPilots.TaskAgents.WebApis;

public sealed class AppConfigure : IServiceRegister, IRequestPiplineRegister, IMvcBuildConfig, IEndpointConfig
{
    public void ConfigureServices(IServiceCollection services, AppBuildContext buildContext)
    {
        services.AddSignalR();
        services.AddTaskAgentsRepositories(buildContext.Configuration);
        services.AddHostedService<SchedulerWorker>();
        services.AddHostedService<RunOrchestratorWorker>();

        services.AddSingleton<IOpenAiClient, FakeOpenAiClient>();
        services.AddSingleton<IMailSender, FakeMailSender>();
        services.AddSingleton<IMailboxPoller, FakeMailboxPoller>();
        services.AddSingleton<IWebhookIngressHandler, FakeWebhookIngressHandler>();
        services.AddSingleton<ITaskAgentsRealtimeNotifier, TaskAgentsRealtimeNotifier>();

        services.AddScoped<SystemApplicationService>();
        services.AddScoped<TaskProvisioningService>();
        services.AddScoped<TaskApplicationService>();
        services.AddScoped<EventApplicationService>();
        services.AddScoped<RunApplicationService>();
        services.AddScoped<ApprovalApplicationService>();
        services.AddScoped<NotificationApplicationService>();
        services.AddScoped<MailboxApplicationService>();
        services.AddScoped<WebhookApplicationService>();
    }

    public RequestPiplineCollection Configure(RequestPiplineCollection pipelineActions, AppBuildContext buildContext)
    {
        pipelineActions.Register("StaticFiles", RequestPipelineStage.BeforeRouting, app =>
        {
            app.UseStaticFiles();
        });

        return pipelineActions;
    }

    public void ConfigureMvcBuilder(IMvcBuilder mvcBuilder, AppBuildContext appBuildContext)
    {
        mvcBuilder.AddNewtonsoftJson(jsonOptions =>
        {
            JsonNetSerializerSettings.DecorateCamelCaseSerializerSettings(jsonOptions.SerializerSettings);
            jsonOptions.SerializerSettings.DateFormatHandling = DateFormatHandling.IsoDateFormat;
            jsonOptions.SerializerSettings.DateTimeZoneHandling = DateTimeZoneHandling.Utc;
            jsonOptions.SerializerSettings.DateFormatString = "O";
            jsonOptions.SerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();
            jsonOptions.SerializerSettings.Converters.Add(new StringEnumConverter());
        });
    }

    public void ConfigureEndpoints(IEndpointRouteBuilder endpoints, AppBuildContext appBuildContext)
    {
        endpoints.MapControllers();
        endpoints.MapHub<TaskAgentsHub>("/agent-hub", options =>
        {
            options.Transports = HttpTransportType.WebSockets;
        });
        endpoints.MapFallbackToFile("index.html");
    }
}
