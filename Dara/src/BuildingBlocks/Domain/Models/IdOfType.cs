using Dara.BuildingBlocks.Domain.Models.Abstraction;

namespace Dara.BuildingBlocks.Domain.Models;

public abstract class IdOfType<T> : IEntityId
{
    public T Value { get; }
    
    public IdOfType(T value)
    {
        Value = value;
    }
}