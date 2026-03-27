using TaskPilots.TaskAgents.Core;
using TaskPilots.TaskAgents.Repositories;

namespace TaskPilots.TaskAgents.WebApis.Services;

public sealed class NotificationApplicationService(INotificationRepository notificationRepository)
{
    public async Task<IReadOnlyList<NotificationListItemDto>> GetNotificationsAsync(CancellationToken cancellationToken = default)
        => (await notificationRepository.ListAsync(cancellationToken: cancellationToken)).Select(x => x.ToListItem()).ToList();
}
