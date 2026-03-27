using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using Microsoft.AspNetCore.SignalR.Client;
using TaskPilots.TaskAgents.Core;
using TaskPilots.TaskAgents.WebApis.Realtime;

namespace TaskPilots.TaskAgents.WebApis.IntegrationTests;

public sealed class SystemApiIntegrationTests
{
    [Fact]
    public async Task GetSystemSummary_ShouldReturnOk()
    {
        await using var fixture = new ApiIntegrationTestFixture();
        await fixture.EnsureStartedAsync();
        using var client = fixture.CreateClient();

        using var response = await client.GetAsync("/api/system/summary");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task PostTask_ThenGetTasks_ShouldIncludeNewTask()
    {
        await using var fixture = new ApiIntegrationTestFixture();
        await fixture.EnsureStartedAsync();
        using var client = fixture.CreateClient();

        var createResponse = await client.PostAsJsonAsync("/api/tasks", new CreateTaskRequest
        {
            Title = "Integration task",
            Description = "Created from integration test.",
            Source = "manual",
            RequiresApproval = false,
        });
        createResponse.EnsureSuccessStatusCode();

        var tasks = await client.GetFromJsonAsync<List<TaskListItemDto>>("/api/tasks");

        Assert.NotNull(tasks);
        Assert.Contains(tasks, task => task.Title == "Integration task");
    }

    [Fact]
    public async Task ApproveSeedApproval_ShouldUpdateTaskStatus()
    {
        await using var fixture = new ApiIntegrationTestFixture();
        await fixture.EnsureStartedAsync();
        using var client = fixture.CreateClient();

        var approvals = await client.GetFromJsonAsync<List<ApprovalListItemDto>>("/api/approvals");
        var approval = Assert.Single(approvals!);

        var approvalResponse = await client.PostAsJsonAsync($"/api/approvals/{approval.ApprovalId}/approve", new ApprovalDecisionRequest
        {
            Note = "Approved from integration test.",
        });
        approvalResponse.EnsureSuccessStatusCode();

        var task = await client.GetFromJsonAsync<TaskDetailDto>($"/api/tasks/{approval.TaskId}");

        Assert.NotNull(task);
        Assert.Equal(nameof(TaskStatus.Completed), task.Status);
    }

    [Fact]
    public async Task GetTasks_ShouldSerializeTimestampsAsIso8601()
    {
        await using var fixture = new ApiIntegrationTestFixture();
        await fixture.EnsureStartedAsync();
        using var client = fixture.CreateClient();

        using var response = await client.GetAsync("/api/tasks");
        response.EnsureSuccessStatusCode();

        var payload = await response.Content.ReadAsStringAsync();
        using var document = JsonDocument.Parse(payload);
        var createdAtUtc = document.RootElement[0].GetProperty("createdAtUtc").GetString();

        Assert.NotNull(createdAtUtc);
        Assert.Contains("T", createdAtUtc);
    }

    [Fact]
    public async Task SignalRHub_ShouldReceiveNotificationAfterTaskCreation()
    {
        await using var fixture = new ApiIntegrationTestFixture();
        await fixture.EnsureStartedAsync();
        using var client = fixture.CreateClient();

        var messages = new List<string>();
        await using var connection = new HubConnectionBuilder()
            .WithUrl(new Uri(fixture.BaseAddress, "/agent-hub"))
            .WithAutomaticReconnect()
            .Build();

        connection.On<RealtimeNotificationPayload>("onNotification", payload => messages.Add(payload.Message));
        await connection.StartAsync();
        await connection.InvokeAsync("SubscribeToWorkspace", "dashboard");

        var createResponse = await client.PostAsJsonAsync("/api/tasks", new CreateTaskRequest
        {
            Title = "Realtime task",
            Description = "Should trigger a realtime event.",
            Source = "manual",
            RequiresApproval = false,
        });
        createResponse.EnsureSuccessStatusCode();

        var timeoutAt = DateTimeOffset.UtcNow.AddSeconds(5);
        while (DateTimeOffset.UtcNow < timeoutAt && messages.Count == 0)
        {
            await Task.Delay(100);
        }

        Assert.NotEmpty(messages);
        await connection.StopAsync();
    }
}
