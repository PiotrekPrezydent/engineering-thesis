namespace Dara.BuildingBlocks.Domain;

public abstract record BaseEntityId(Guid Value) : IEntityId;
