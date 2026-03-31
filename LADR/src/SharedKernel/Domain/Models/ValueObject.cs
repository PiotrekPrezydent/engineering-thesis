using System.Reflection;

namespace LADR.SharedKernel.Domain.Models;

public abstract class ValueObject : IEquatable<ValueObject>
{
    List<PropertyInfo> _properties;
    List<FieldInfo> _fields;
    
    public bool Equals(ValueObject? other)
    {
        if (other is null)
            return false;
        
        if (ReferenceEquals(this, other)) 
            return true;
        
        return GetProperties().All(p=> Equals(p.GetValue(this, null), p.GetValue(other,null))) &&
               GetFields().All(f=> Equals(f.GetValue(this), f.GetValue(this)));
    }
    
    public override bool Equals(object? obj)
    {
        return Equals(obj as ValueObject);
    }
    
    public static bool operator ==(ValueObject? left, ValueObject? right)
    {
        if (left is null) return right is null;
        
        return left.Equals(right);
    }
    
    public static bool operator !=(ValueObject? left, ValueObject? right)
    {
        return !(left == right);
    }
    
    List<PropertyInfo> GetProperties()
    {
        if (_properties != null)
            return _properties;
        
        _properties = GetType().GetProperties(BindingFlags.Instance | BindingFlags.Public).ToList();
        return _properties;
    }

    List<FieldInfo> GetFields()
    {
        if (_fields != null)
            return _fields;
        
        _fields = GetType().GetFields(BindingFlags.Instance | BindingFlags.Public).ToList();
        return _fields;
    }
    
}