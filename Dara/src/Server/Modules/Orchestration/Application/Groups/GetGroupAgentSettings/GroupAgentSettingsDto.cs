namespace Dara.Server.Modules.Orchestration.Application.Groups.GetGroupAgentSettings;

public record GroupAgentSettingsDto(
    Guid GroupId,
    Guid OwnerId,
    string PromptMessagePrefix,
    string ChatClientProviderName,
    string ClientModelName,
    string? ApiKey);
