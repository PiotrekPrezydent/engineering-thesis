using Dara.Server.BuildingBlocks.Domain.Events;
using Dara.Server.Modules.Groups.Domain.Groups;

namespace Dara.Server.Modules.Groups.Domain.GroupMessages.Events;

public record NewGroupMessageCreatedDomainEvent(GroupMessageId MessageId, GroupId GroupId, GroupMemberId MemberId, string Content) : DomainEventBase;