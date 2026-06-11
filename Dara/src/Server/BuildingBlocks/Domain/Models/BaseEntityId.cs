namespace Dara.BuildingBlocks.Domain.Models;

public abstract class BaseEntityId
{
    public Guid Value { get; }
    
    protected BaseEntityId(Guid value)
    {
        Value = value;
    }
}