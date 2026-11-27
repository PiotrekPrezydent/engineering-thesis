using Dara.Server.BuildingBlocks.Domain.Events;

namespace Dara.Server.Modules.Identity.Domain.Users.Events;

public record NewUserCreatedDomainEvent(UserId UserId) : DomainEventBase;