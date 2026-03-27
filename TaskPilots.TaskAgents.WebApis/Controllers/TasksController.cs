using Microsoft.AspNetCore.Mvc;
using TaskPilots.TaskAgents.Core;
using TaskPilots.TaskAgents.WebApis.Services;

namespace TaskPilots.TaskAgents.WebApis.Controllers;

[ApiController]
[Route("api/tasks")]
public sealed class TasksController(TaskApplicationService applicationService) : ControllerBase
{
    [HttpGet]
    [ProducesResponseType<IReadOnlyList<TaskListItemDto>>(StatusCodes.Status200OK)]
    public Task<IReadOnlyList<TaskListItemDto>> GetTasks(CancellationToken cancellationToken)
        => applicationService.GetTasksAsync(cancellationToken);

    [HttpGet("{taskId}")]
    [ProducesResponseType<TaskDetailDto>(StatusCodes.Status200OK)]
    [ProducesResponseType<ApiErrorResponse>(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<TaskDetailDto>> GetTask(string taskId, CancellationToken cancellationToken)
    {
        var task = await applicationService.GetTaskAsync(taskId, cancellationToken);
        if (task is null)
        {
            return NotFound(new ApiErrorResponse("Task not found.", "task_not_found"));
        }

        return Ok(task);
    }

    [HttpPost]
    [ProducesResponseType<TaskDetailDto>(StatusCodes.Status201Created)]
    [ProducesResponseType<ApiErrorResponse>(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<TaskDetailDto>> CreateTask([FromBody] CreateTaskRequest request, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(request.Title))
        {
            return BadRequest(new ApiErrorResponse("Task title is required.", "task_title_required"));
        }

        var task = await applicationService.CreateTaskAsync(request, cancellationToken);
        return CreatedAtAction(nameof(GetTask), new { taskId = task.TaskId }, task);
    }
}
