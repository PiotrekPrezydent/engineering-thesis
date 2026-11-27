using Dara.Server.BuildingBlocks.Domain;

namespace Dara.Server.Modules.Authorization.Domain.Users.Clients;

// the id of a client of the account given by Identity
public record UserClientId(Guid Value) : BaseEntityId(Value);
