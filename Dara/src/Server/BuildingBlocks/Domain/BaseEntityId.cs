namespace Dara.Server.BuildingBlocks.Domain;

public abstract record BaseEntityId(Guid Value) : IEntityId
{
    public static implicit operator Guid(BaseEntityId id) => id.Value;
    
}
