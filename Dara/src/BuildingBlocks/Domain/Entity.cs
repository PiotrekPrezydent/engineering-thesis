using Dara.BuildingBlocks.Domain.Events.Abstractions;

namespace Dara.BuildingBlocks.Domain;

public abstract class Entity : IEquatable<Entity>
{
    public Guid Id { get; protected set; }
    
    public IReadOnlyCollection<IDomainEvent> DomainEvents => _events.AsReadOnly();
    protected readonly List<IDomainEvent> _events = new();
    
    public void ClearDomainEvents()
    {
        _events.Clear();
    }

    public bool Equals(Entity? other)
    {
        if (other is null)
            return false;
        
        if (ReferenceEquals(this, other)) 
            return true;
        
        if (this.GetType() != other.GetType()) 
            return false;
        
        return this.Id == other.Id;
    }

    public override bool Equals(object? obj)
    {
        return Equals(obj as Entity);
    }
    
    //id should not be used here because it's not read only, but idk how to fix it now
    public override int GetHashCode()
    {
        return HashCode.Combine(Id, this.GetType());
    }
    
    public static bool operator ==(Entity? left, Entity? right)
    {
        if (left is null) return right is null;
        
        return left.Equals(right);
    }
    
    public static bool operator !=(Entity? left, Entity? right)
    {
        return !(left == right);
    }
}