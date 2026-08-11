namespace Dara.Server.BuildingBlocks.Domain;

public abstract record BaseEntityId(Guid Value) : IEntityId
{
    public static implicit operator Guid(BaseEntityId id) => id.Value;
    
    public bool Match(Guid guid) => Value == guid;

    public bool MatchAny(IEnumerable<Guid> guids) => guids.Any(guid => Value == guid);
    
}
