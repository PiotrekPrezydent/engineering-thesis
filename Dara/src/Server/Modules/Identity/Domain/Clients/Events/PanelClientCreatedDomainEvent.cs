using Dara.Server.BuildingBlocks.Domain.Events;
using Dara.Server.Modules.Identity.Domain.Users;

namespace Dara.Server.Modules.Identity.Domain.Clients.Events;

// the new account and its panel client, created together; the application publishes the new account with the id of
// its panel, which holds the roles of the account panel
public record PanelClientCreatedDomainEvent(UserId UserId, ClientId ClientId) : DomainEventBase;
