using TaskPilots.TaskAgents.Core;
using TaskPilots.TaskAgents.WebApis.Services;

namespace TaskPilots.TaskAgents.WebApis.Tests;

public sealed class ApprovalApplicationServiceTests
{
    [Fact]
    public async Task ApproveAsync_ShouldCompleteRunAndTask()
    {
        await using var provider = TestServiceCollectionFactory.CreateProvider();
        var service = provider.GetRequiredService<ApprovalApplicationService>();
        var taskService = provider.GetRequiredService<TaskApplicationService>();

        var approval = Assert.Single(await service.GetApprovalsAsync());
        await service.ApproveAsync(approval.ApprovalId, new ApprovalDecisionRequest
        {
            Note = "Approved in test.",
        });

        var task = await taskService.GetTaskAsync(approval.TaskId);
        Assert.NotNull(task);
        Assert.Equal(nameof(TaskStatus.Completed), task.Status);
    }
}
