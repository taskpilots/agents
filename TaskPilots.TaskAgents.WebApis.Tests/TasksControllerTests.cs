using Microsoft.AspNetCore.Mvc;
using TaskPilots.TaskAgents.Core;
using TaskPilots.TaskAgents.WebApis.Controllers;
using TaskPilots.TaskAgents.WebApis.Services;

namespace TaskPilots.TaskAgents.WebApis.Tests;

public sealed class TasksControllerTests
{
    [Fact]
    public async Task CreateTask_WhenValidRequest_ShouldReturnCreated()
    {
        await using var provider = TestServiceCollectionFactory.CreateProvider();
        var service = provider.GetRequiredService<TaskApplicationService>();
        var controller = new TasksController(service);

        var result = await controller.CreateTask(new CreateTaskRequest
        {
            Title = "Controller test task",
            Description = "Creates a synthetic task.",
            Source = "manual",
            RequiresApproval = false,
        }, CancellationToken.None);

        var created = Assert.IsType<CreatedAtActionResult>(result.Result);
        var payload = Assert.IsType<TaskDetailDto>(created.Value);
        Assert.Equal("Controller test task", payload.Title);
    }

    [Fact]
    public async Task CreateTask_WhenTitleMissing_ShouldReturnBadRequest()
    {
        await using var provider = TestServiceCollectionFactory.CreateProvider();
        var service = provider.GetRequiredService<TaskApplicationService>();
        var controller = new TasksController(service);

        var result = await controller.CreateTask(new CreateTaskRequest(), CancellationToken.None);

        var badRequest = Assert.IsType<BadRequestObjectResult>(result.Result);
        var payload = Assert.IsType<ApiErrorResponse>(badRequest.Value);
        Assert.Equal("task_title_required", payload.Code);
    }
}
