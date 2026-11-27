using Dara.Server.BuildingBlocks.Domain.Events;
using Dara.Server.Modules.Identity.Domain.Users.Clients;

namespace Dara.Server.Modules.Identity.Domain.Users.Events;

public record UserClientContextChangedDomainEvent(UserId UserId, UserClientId ClientId, ClientContext Context) : DomainEventBase;