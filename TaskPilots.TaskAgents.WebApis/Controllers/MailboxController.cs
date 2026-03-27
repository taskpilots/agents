using Microsoft.AspNetCore.Mvc;
using TaskPilots.TaskAgents.Core;
using TaskPilots.TaskAgents.WebApis.Services;

namespace TaskPilots.TaskAgents.WebApis.Controllers;

[ApiController]
[Route("api/mailbox")]
public sealed class MailboxController(MailboxApplicationService applicationService) : ControllerBase
{
    [HttpGet("messages")]
    [ProducesResponseType<IReadOnlyList<MailboxMessageListItemDto>>(StatusCodes.Status200OK)]
    public Task<IReadOnlyList<MailboxMessageListItemDto>> GetMessages(CancellationToken cancellationToken)
        => applicationService.GetMessagesAsync(cancellationToken);

    [HttpPost("simulate-ingest")]
    [ProducesResponseType<MailboxMessageListItemDto>(StatusCodes.Status200OK)]
    public Task<MailboxMessageListItemDto> SimulateIngress([FromBody] SimulateMailboxIngestRequest request, CancellationToken cancellationToken)
        => applicationService.SimulateIngressAsync(request, cancellationToken);
}
