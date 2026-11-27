using Dara.Server.BuildingBlocks.Domain.Events;
using Dara.Server.Modules.Authorization.Domain.Users.Clients;

namespace Dara.Server.Modules.Authorization.Domain.Users.Events;

public record ClientRoleRemovedDomainEvent(UserId UserId, UserClientId ClientId, string RoleCode) : DomainEventBase;
