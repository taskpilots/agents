using TaskPilots.TaskAgents.Core;
using TaskPilots.TaskAgents.Repositories;

namespace TaskPilots.TaskAgents.WebApis.Tests;

public sealed class InMemoryRepositoriesTests
{
    [Fact]
    public async Task InMemoryTaskRepository_ShouldPersistAndUpdateTask()
    {
        var store = new InMemoryTaskAgentsStore();
        store.WithLock(() => store.Tasks.Clear());
        var repository = new InMemoryTaskRepository(store);
        var task = new TaskRecord(
            "task-1",
            "Test task",
            "manual",
            TaskStatus.Created,
            "summary",
            "medium",
            false,
            DateTimeOffset.UtcNow,
            DateTimeOffset.UtcNow,
            null,
            null,
            null);

        await repository.AddAsync(task);
        await repository.UpdateAsync(task with { Status = TaskStatus.Completed });

        var loaded = await repository.GetAsync("task-1");

        Assert.NotNull(loaded);
        Assert.Equal(TaskStatus.Completed, loaded.Status);
    }
}
