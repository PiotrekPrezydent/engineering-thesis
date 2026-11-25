using Dara.Server.BuildingBlocks.Application.Queries;

namespace Dara.Server.Modules.Orchestration.Application.Participants.GetParticipantFunctions;

// empty when the participant is not projected yet
public record GetParticipantFunctionsQuery(Guid ParticipantId) : IQuery<List<ParticipantFunctionDto>>;
