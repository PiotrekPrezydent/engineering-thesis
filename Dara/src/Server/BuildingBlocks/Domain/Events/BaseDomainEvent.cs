using Dara.BuildingBlocks.Domain.Models;

namespace Dara.BuildingBlocks.Domain.Events;

public record BaseDomainEvent<TId>(TId EventOwnerId) : IDomainEvent where TId : BaseEntityId;