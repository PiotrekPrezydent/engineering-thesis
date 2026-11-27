using Dara.Server.BuildingBlocks.Domain.Events;

namespace Dara.Server.Modules.Authorization.Domain.Users.Events;

public record AccountRoleRemovedDomainEvent(UserId UserId, string RoleCode) : DomainEventBase;
