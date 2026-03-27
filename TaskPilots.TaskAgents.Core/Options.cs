namespace TaskPilots.TaskAgents.Core;

public sealed record OpenAiOptions
{
    public string BaseUrl { get; init; } = "https://api.openai.com/v1";
    public string Model { get; init; } = "gpt-5-mini";
    public string ApiKey { get; init; } = string.Empty;
}

public sealed record MailboxOptions
{
    public bool Enabled { get; init; }
    public string PollInterval { get; init; } = "00:00:30";
    public string InboxAddress { get; init; } = "analysis@example.local";
}

public sealed record WebhookOptions
{
    public bool Enabled { get; init; } = true;
    public string SharedSecret { get; init; } = "development-secret";
}

public sealed record StorageOptions
{
    public string RootPath { get; init; } = "./data";
    public string ArchivePath { get; init; } = "./data/archive";
}

public sealed record TaskAgentsDataOptions
{
    public string Mode { get; init; } = "InMemory";
}
