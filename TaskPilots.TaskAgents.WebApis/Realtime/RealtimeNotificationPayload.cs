namespace TaskPilots.TaskAgents.WebApis.Realtime;

public sealed record RealtimeNotificationPayload(string Message, DateTimeOffset OccurredAtUtc);
