using System.Linq;
using Dara.Shared.SourceGenerators.BuilderClass.Models;
using Dara.Shared.SourceGenerators.Common;

namespace Dara.Shared.SourceGenerators.BuilderClass;

public record BuilderClassSpecification(
    string Namespace,
    string ClassName,
    EquatableArray<PropertyData> Properties)
{
    public string BuilderClassName =>  ClassName+"Builder";
    public string ClassInstanceFieldName => "_instance";
    
    public virtual bool Equals(BuilderClassSpecification? other)
    {
        if (other is null) return false;
        
        return ClassName == other.ClassName &&
               Namespace == other.Namespace &&
               Properties.SequenceEqual(other.Properties);
    }
    
    public override int GetHashCode()
    {
        int hash = ClassName.GetHashCode() ^ Namespace.GetHashCode();
        foreach (var prop in Properties)
        {
            hash ^= prop.GetHashCode();
        }
        return hash;
    }
}
