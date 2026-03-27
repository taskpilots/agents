using TaskPilots.TaskAgents.Core;
using TaskPilots.TaskAgents.Repositories;

namespace TaskPilots.TaskAgents.WebApis.Services;

public sealed class EventApplicationService(IEventRepository eventRepository)
{
    public async Task<IReadOnlyList<TaskEventDto>> GetEventsAsync(string? taskId = null, CancellationToken cancellationToken = default)
        => (await eventRepository.ListAsync(taskId, cancellationToken)).Select(x => x.ToDto()).ToList();
}
