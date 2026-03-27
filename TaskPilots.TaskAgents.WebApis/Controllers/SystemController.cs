using Microsoft.AspNetCore.Mvc;
using TaskPilots.TaskAgents.Core;
using TaskPilots.TaskAgents.WebApis.Services;

namespace TaskPilots.TaskAgents.WebApis.Controllers;

[ApiController]
[Route("api/system")]
public sealed class SystemController(SystemApplicationService applicationService) : ControllerBase
{
    [HttpGet("summary")]
    [ProducesResponseType<SystemSummaryDto>(StatusCodes.Status200OK)]
    public Task<SystemSummaryDto> GetSummary(CancellationToken cancellationToken)
        => applicationService.GetSummaryAsync(cancellationToken);
}
