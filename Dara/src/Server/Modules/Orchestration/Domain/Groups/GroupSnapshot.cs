namespace Dara.Server.Modules.Orchestration.Domain.Groups;

public record GroupSnapshot(Guid GroupId, string PromptMessagePrefix, string ChatClientProviderName, string ClientModelName,  string ApiKey, IReadOnlyList<Guid> ParticipantsIds);