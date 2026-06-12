using Dara.Server.BuildingBlocks.Domain;

namespace Dara.Server.Modules.Identity.Domain.Events;

public record NewUserRegisteredDomainEvent(UserId UserId) : IDomainEvent;