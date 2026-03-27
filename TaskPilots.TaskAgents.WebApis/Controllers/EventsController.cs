using Microsoft.AspNetCore.Mvc;
using TaskPilots.TaskAgents.Core;
using TaskPilots.TaskAgents.WebApis.Services;

namespace TaskPilots.TaskAgents.WebApis.Controllers;

[ApiController]
[Route("api/events")]
public sealed class EventsController(EventApplicationService applicationService) : ControllerBase
{
    [HttpGet]
    [ProducesResponseType<IReadOnlyList<TaskEventDto>>(StatusCodes.Status200OK)]
    public Task<IReadOnlyList<TaskEventDto>> GetEvents([FromQuery] string? taskId, CancellationToken cancellationToken)
        => applicationService.GetEventsAsync(taskId, cancellationToken);
}
