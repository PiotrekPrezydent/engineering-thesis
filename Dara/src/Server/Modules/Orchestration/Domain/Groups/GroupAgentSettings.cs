using Dara.Server.BuildingBlocks.Domain;

namespace Dara.Server.Modules.Orchestration.Domain.Groups;

// how the group talks to the model, without the api key kept by the group itself
public record GroupAgentSettings(
    string PromptMessagePrefix,
    string ChatClientProviderName,
    string ClientModelName) : IValueObject;
