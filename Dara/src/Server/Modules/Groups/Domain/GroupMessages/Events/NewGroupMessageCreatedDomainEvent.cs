using Dara.Server.BuildingBlocks.Domain.Events;
using Dara.Server.Modules.Groups.Domain.Groups;
using Dara.Server.Modules.Groups.Domain.Members;

namespace Dara.Server.Modules.Groups.Domain.GroupMessages.Events;

public record NewGroupMessageCreatedDomainEvent(GroupMessageId MessageId, GroupId GroupId, MemberId AuthorId, string Content) : DomainEventBase;