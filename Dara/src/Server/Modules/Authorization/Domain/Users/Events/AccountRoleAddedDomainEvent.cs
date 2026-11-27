using Dara.Server.BuildingBlocks.Domain.Events;

namespace Dara.Server.Modules.Authorization.Domain.Users.Events;

public record AccountRoleAddedDomainEvent(UserId UserId, string RoleCode) : DomainEventBase;
