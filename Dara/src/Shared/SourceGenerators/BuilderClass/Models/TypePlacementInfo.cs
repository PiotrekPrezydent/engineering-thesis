using System.Collections.Generic;

namespace Dara.Shared.SourceGenerators.BuilderClass.Models;

public record TypePlacementInfo(
    TypeData Data,
    TypePlacementInfo? ParentInfo,
    int GenericParameterIndex = -1
)
{
    public bool IsGeneric => Data is GenericTypeData;
    public bool IsGenericParameter => GenericParameterIndex != -1;
    public bool IsRoot => ParentInfo is null;
    
    public IEnumerable<TypePlacementInfo> Ancestors
    {
        get
        {
            var current = ParentInfo;
            while (current != null)
            {
                yield return current;
                current = current.ParentInfo;
            }
        }
    }
}