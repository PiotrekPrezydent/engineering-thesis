using Dara.Server.BuildingBlocks.Domain.Events;
using Dara.Server.Modules.Identity.Domain.Users;

namespace Dara.Server.Modules.Identity.Domain.Clients.Events;

// a client of the hub only; the panel client of an account gives PanelClientCreatedDomainEvent
public record UserClientRegisteredDomainEvent(UserId UserId, ClientId ClientId, string Name, ClientType Type) : DomainEventBase;
