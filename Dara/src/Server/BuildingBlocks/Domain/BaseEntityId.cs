namespace Dara.Server.BuildingBlocks.Domain;

public abstract record BaseEntityId(Guid Value) : IEntityId;
