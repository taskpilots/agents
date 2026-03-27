using TaskPilots.TaskAgents.Core;
using TaskPilots.TaskAgents.Repositories;

namespace TaskPilots.TaskAgents.WebApis.Services;

public sealed class TaskApplicationService(
    ITaskRepository taskRepository,
    IEventRepository eventRepository,
    IRunRepository runRepository,
    IApprovalRepository approvalRepository,
    TaskProvisioningService provisioningService)
{
    public async Task<IReadOnlyList<TaskListItemDto>> GetTasksAsync(CancellationToken cancellationToken = default)
        => (await taskRepository.ListAsync(cancellationToken)).Select(x => x.ToListItem()).ToList();

    public async Task<TaskDetailDto?> GetTaskAsync(string taskId, CancellationToken cancellationToken = default)
    {
        var task = await taskRepository.GetAsync(taskId, cancellationToken);
        if (task is null)
        {
            return null;
        }

        var events = await eventRepository.ListAsync(taskId, cancellationToken);
        var runs = await runRepository.ListByTaskIdAsync(taskId, cancellationToken);
        var approvals = await approvalRepository.ListAsync(taskId, cancellationToken);

        return new TaskDetailDto(
            task.TaskId,
            task.Title,
            task.Source,
            task.Status.ToString(),
            task.Summary,
            task.Priority,
            task.RequiresApproval,
            task.CreatedAtUtc,
            task.UpdatedAtUtc,
            events.Select(x => x.ToDto()).ToList(),
            runs.Select(x => x.ToListItem()).ToList(),
            approvals.Select(x => x.ToListItem()).ToList());
    }

    public async Task<TaskDetailDto> CreateTaskAsync(CreateTaskRequest request, CancellationToken cancellationToken = default)
    {
        var title = request.Title.Trim();
        if (string.IsNullOrWhiteSpace(title))
        {
            throw new ArgumentException("Task title is required.", nameof(request));
        }

        var task = await provisioningService.CreateTaskAsync(
            title,
            string.IsNullOrWhiteSpace(request.Description) ? "No description supplied." : request.Description.Trim(),
            string.IsNullOrWhiteSpace(request.Source) ? "manual" : request.Source.Trim(),
            request.RequiresApproval,
            cancellationToken: cancellationToken);

        return (await GetTaskAsync(task.TaskId, cancellationToken))!;
    }
}
