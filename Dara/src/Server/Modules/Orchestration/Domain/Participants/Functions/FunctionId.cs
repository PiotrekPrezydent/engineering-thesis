using Dara.Server.BuildingBlocks.Domain;

namespace Dara.Server.Modules.Orchestration.Domain.Participants.Functions;

public record FunctionId(Guid Value) : BaseEntityId(Value);