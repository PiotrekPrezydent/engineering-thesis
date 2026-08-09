using Dara.Server.BuildingBlocks.Domain.Events;

namespace Dara.Server.Modules.Identity.Domain.Events;

public record NewClientCreatedDomainEvent(ClientId ClientId) : DomainEventBase;