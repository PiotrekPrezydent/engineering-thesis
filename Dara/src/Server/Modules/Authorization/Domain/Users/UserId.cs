using Dara.Server.BuildingBlocks.Domain;

namespace Dara.Server.Modules.Authorization.Domain.Users;

// the id of the account given by Identity
public record UserId(Guid Value) : BaseEntityId(Value);
