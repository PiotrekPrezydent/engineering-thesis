using Dara.Server.BuildingBlocks.Domain.Events;
using Dara.Server.Modules.Identity.Domain.Users;

namespace Dara.Server.Modules.Identity.Domain.Clients.Events;

public record UserClientRenamedDomainEvent(UserId UserId, ClientId ClientId, string Name) : DomainEventBase;
