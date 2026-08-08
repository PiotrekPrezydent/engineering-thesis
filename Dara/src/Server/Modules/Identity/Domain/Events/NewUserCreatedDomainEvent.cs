using Dara.Server.BuildingBlocks.Domain;
using Dara.Server.BuildingBlocks.Domain.Events;

namespace Dara.Server.Modules.Identity.Domain.Events;

public record NewUserCreatedDomainEvent(UserId UserId) : DomainEventBase;