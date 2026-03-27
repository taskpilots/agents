using Microsoft.AspNetCore.Mvc;
using TaskPilots.TaskAgents.Core;
using TaskPilots.TaskAgents.WebApis.Services;

namespace TaskPilots.TaskAgents.WebApis.Controllers;

[ApiController]
[Route("api/runs")]
public sealed class RunsController(RunApplicationService applicationService) : ControllerBase
{
    [HttpGet]
    [ProducesResponseType<IReadOnlyList<AgentRunListItemDto>>(StatusCodes.Status200OK)]
    public Task<IReadOnlyList<AgentRunListItemDto>> GetRuns(CancellationToken cancellationToken)
        => applicationService.GetRunsAsync(cancellationToken);

    [HttpGet("{runId}")]
    [ProducesResponseType<AgentRunDetailDto>(StatusCodes.Status200OK)]
    [ProducesResponseType<ApiErrorResponse>(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<AgentRunDetailDto>> GetRun(string runId, CancellationToken cancellationToken)
    {
        var run = await applicationService.GetRunAsync(runId, cancellationToken);
        if (run is null)
        {
            return NotFound(new ApiErrorResponse("Run not found.", "run_not_found"));
        }

        return Ok(run);
    }

    [HttpPost("{runId}/retry")]
    [ProducesResponseType<AgentRunDetailDto>(StatusCodes.Status200OK)]
    [ProducesResponseType<ApiErrorResponse>(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<AgentRunDetailDto>> RetryRun(string runId, CancellationToken cancellationToken)
    {
        var run = await applicationService.RetryRunAsync(runId, cancellationToken);
        if (run is null)
        {
            return NotFound(new ApiErrorResponse("Run not found.", "run_not_found"));
        }

        return Ok(run);
    }
}
