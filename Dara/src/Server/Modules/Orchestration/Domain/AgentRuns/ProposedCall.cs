using Dara.Server.BuildingBlocks.Domain;

namespace Dara.Server.Modules.Orchestration.Domain.AgentRuns;

// a call the model asked for: the tool alias and the arguments as one json object
public record ProposedCall(string Alias, string ArgumentsJson) : IValueObject;
