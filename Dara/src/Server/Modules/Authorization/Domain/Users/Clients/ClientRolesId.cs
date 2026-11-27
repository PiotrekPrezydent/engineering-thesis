using Dara.Server.BuildingBlocks.Domain;

namespace Dara.Server.Modules.Authorization.Domain.Users.Clients;

public record ClientRolesId(Guid Value) : BaseEntityId(Value);
