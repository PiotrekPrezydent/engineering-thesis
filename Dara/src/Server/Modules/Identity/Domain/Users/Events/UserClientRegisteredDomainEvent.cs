using Dara.Server.BuildingBlocks.Domain.Events;
using Dara.Server.Modules.Identity.Domain.Users.Clients;

namespace Dara.Server.Modules.Identity.Domain.Users.Events;

public record UserClientRegisteredDomainEvent(UserId UserId, UserClientId ClientId, string Name, ClientType Type) : DomainEventBase;