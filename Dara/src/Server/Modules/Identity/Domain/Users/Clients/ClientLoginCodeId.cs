using Dara.Server.BuildingBlocks.Domain;

namespace Dara.Server.Modules.Identity.Domain.Users.Clients;

public record ClientLoginCodeId(Guid Value) : BaseEntityId(Value);