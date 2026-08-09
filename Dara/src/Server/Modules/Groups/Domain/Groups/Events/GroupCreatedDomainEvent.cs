using Dara.Server.BuildingBlocks.Domain.Events;

namespace Dara.Server.Modules.Groups.Domain.Groups.Events;

public record GroupCreatedDomainEvent(GroupId GroupId, GroupMemberId OwnerId) : DomainEventBase;