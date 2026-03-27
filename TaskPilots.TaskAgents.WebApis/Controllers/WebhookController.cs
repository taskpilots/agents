using Microsoft.AspNetCore.Mvc;
using TaskPilots.TaskAgents.Core;
using TaskPilots.TaskAgents.WebApis.Services;

namespace TaskPilots.TaskAgents.WebApis.Controllers;

[ApiController]
[Route("api/webhooks")]
public sealed class WebhookController(WebhookApplicationService applicationService) : ControllerBase
{
    [HttpPost("inbound")]
    [ProducesResponseType<TaskDetailDto>(StatusCodes.Status200OK)]
    public Task<TaskDetailDto> Inbound([FromBody] WebhookInboundRequest request, CancellationToken cancellationToken)
        => applicationService.HandleAsync(request, cancellationToken);
}
