using Dara.Server.BuildingBlocks.Domain;

namespace Dara.Server.Modules.Clients.Domain.Clients.Events;

public record ClientNameChangedDomainEvent(ClientId ClientId,  string NewName) : IDomainEvent;