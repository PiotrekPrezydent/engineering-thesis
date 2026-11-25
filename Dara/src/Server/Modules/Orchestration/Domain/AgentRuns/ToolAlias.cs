using Dara.Server.BuildingBlocks.Domain;
using Dara.Server.Modules.Orchestration.Domain.Participants;
using Dara.Server.Modules.Orchestration.Domain.Participants.Functions;

namespace Dara.Server.Modules.Orchestration.Domain.AgentRuns;

// the tool name the model sees for one participant function in one run
public record ToolAlias(string Alias, ParticipantId ParticipantId, FunctionId FunctionId) : IValueObject;
