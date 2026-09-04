using Dara.Server.BuildingBlocks.Domain;

namespace Dara.Server.Modules.Orchestration.Domain.Groups;

public record GroupId(Guid Value) : BaseEntityId(Value);