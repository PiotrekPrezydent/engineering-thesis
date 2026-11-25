namespace Dara.Server.Modules.Orchestration.Domain.Groups;

// what an agent run keeps of its group, without the api key: the key is read from the group when the model is called
public record GroupSnapshot(Guid GroupId, Guid OwnerId, string PromptMessagePrefix, string ChatClientProviderName, string ClientModelName, IReadOnlyList<Guid> ParticipantsIds);
