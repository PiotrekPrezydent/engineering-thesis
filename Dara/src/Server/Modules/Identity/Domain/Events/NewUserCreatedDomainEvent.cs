using Dara.Server.BuildingBlocks.Domain;

namespace Dara.Server.Modules.Identity.Domain.Events;

public record NewUserCreatedDomainEvent(UserId UserId) : IDomainEvent;