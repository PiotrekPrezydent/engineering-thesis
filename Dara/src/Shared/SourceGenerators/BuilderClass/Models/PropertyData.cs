using System.Linq;
using Dara.Shared.SourceGenerators.Common;
using Dara.Shared.SourceGenerators.Extensions;

namespace Dara.Shared.SourceGenerators.BuilderClass.Models;

public record PropertyData(
    string Name,
    TypeData Type,
    EquatableArray<PropertyAttributeData> Attributes)
{
    public string NameAsParameter => Name.FirstCharToLower();
    
    public virtual bool Equals(PropertyData? other)
    {
        return Name == other?.Name &&
               Type == other?.Type &&
               Attributes.SequenceEqual(other?.Attributes);
    }
    
    public override int GetHashCode()
    {
        int hash = Name.GetHashCode() ^ Type.GetHashCode();
        foreach (var atr in Attributes)
        {
            hash ^= atr.GetHashCode();
        }

        return hash;
    }
}