using AgileLabs.WebApp;
using Microsoft.AspNetCore.Mvc;

namespace TaskPilots.TaskAgents.WebApis;

public sealed class TaskAgentsAppOptions : DefaultMvcApplicationOptions
{
    public TaskAgentsAppOptions()
    {
        TypeFinderAssemblyScanPattern = "^TaskPilots|^AgileLabs";
        MvcBuilderCreateFunc = static (services, action) => services.AddControllersWithViews(action);
    }
}
