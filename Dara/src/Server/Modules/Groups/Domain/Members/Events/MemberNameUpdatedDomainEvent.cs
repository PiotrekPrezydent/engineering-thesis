using Dara.Server.BuildingBlocks.Domain.Events;

namespace Dara.Server.Modules.Groups.Domain.Members.Events;

public record MemberNameUpdatedDomainEvent(MemberId Id, string NewName) : DomainEventBase;