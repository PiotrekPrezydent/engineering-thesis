using Dara.Server.BuildingBlocks.Domain;

namespace Dara.Server.Modules.Profiles.Domain;

public record ProfileId(Guid Value) : BaseEntityId(Value);