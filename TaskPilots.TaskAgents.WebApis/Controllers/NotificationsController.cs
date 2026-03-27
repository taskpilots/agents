using Microsoft.AspNetCore.Mvc;
using TaskPilots.TaskAgents.Core;
using TaskPilots.TaskAgents.WebApis.Services;

namespace TaskPilots.TaskAgents.WebApis.Controllers;

[ApiController]
[Route("api/notifications")]
public sealed class NotificationsController(NotificationApplicationService applicationService) : ControllerBase
{
    [HttpGet]
    [ProducesResponseType<IReadOnlyList<NotificationListItemDto>>(StatusCodes.Status200OK)]
    public Task<IReadOnlyList<NotificationListItemDto>> GetNotifications(CancellationToken cancellationToken)
        => applicationService.GetNotificationsAsync(cancellationToken);
}
