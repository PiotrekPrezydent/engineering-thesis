using Dara.Server.BuildingBlocks.Domain.Events;

namespace Dara.Server.Modules.Groups.Domain.Groups.Events;

public record GroupMemberJoinedDomainEvent(GroupId GroupId, GroupMemberId MemberId) : DomainEventBase;