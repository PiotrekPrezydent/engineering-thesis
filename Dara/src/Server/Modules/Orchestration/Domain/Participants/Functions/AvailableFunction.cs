using Dara.Server.BuildingBlocks.Domain;
using Dara.Server.Modules.Orchestration.Domain.Participants.Functions.Descriptions;

namespace Dara.Server.Modules.Orchestration.Domain.Participants.Functions;

// a plugin function the participant can use now, the id is the plugin function id
public record AvailableFunction(FunctionId Id, FunctionDescription Description) : IValueObject;
