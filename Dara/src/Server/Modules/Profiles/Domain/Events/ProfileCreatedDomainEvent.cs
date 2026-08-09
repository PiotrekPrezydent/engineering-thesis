using Dara.Server.BuildingBlocks.Domain.Events;

namespace Dara.Server.Modules.Profiles.Domain.Events;

public record ProfileCreatedDomainEvent(ProfileId ProfileId) : DomainEventBase;