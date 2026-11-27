using Dara.Server.BuildingBlocks.Domain;

namespace Dara.Server.Modules.Identity.Domain.Clients;

public record ClientLoginCodeId(Guid Value) : BaseEntityId(Value);