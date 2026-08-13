using Dara.Server.BuildingBlocks.Domain.Events;
using Dara.Server.Modules.Groups.Domain.Members;

namespace Dara.Server.Modules.Groups.Domain.Groups.Events;

public record NewMemberJoinedGroupDomainEvent(GroupId GroupId, MemberId MemberId) : DomainEventBase;