namespace Dara.Shared.Contracts.StateSnapshots;

// the api key only for the group owner
public record GroupAgentSettingsSnapshot(Guid GroupId, Guid OwnerId, string PromptMessagePrefix, string ChatClientProviderName, string ClientModelName, string? ApiKey);
