using Dara.Shared.SourceGenerators.Common;

namespace Dara.Shared.SourceGenerators.BuilderClass.Models;

public abstract record TypeData(string Name, string Namespace, GenericConstraints SatisfiedGenericConstraints)
{
    public virtual string FullName => $"{Namespace}.{Name}";
    
    public bool Match(TypeData targetType)
    {
        return Name == targetType.Name;
    }

    public bool TryReplaceDataAsGenericType(TypeData targetType, string parameterName, out string typeFullNameAsGeneric, out string whereStatement)
    {
        typeFullNameAsGeneric = "";
        whereStatement = "";
        var placement = this.GetPlacementInfoForType(targetType);
        
        if (placement == null)
            return false;
        
        if (placement.IsGenericParameter || !placement.IsRoot) 
        {
            string reconstructedTypeName = parameterName; 
            
            var currentPlacement = placement;
            var index = currentPlacement.GenericParameterIndex;
            if (currentPlacement.ParentInfo?.Data is GenericTypeData parent)
                whereStatement = parent.GetWhereStatemtForArgument(parameterName, index);
            
            while (currentPlacement.ParentInfo != null)
            {
                var parentInfo = currentPlacement.ParentInfo;
                int parameterIndex = currentPlacement.GenericParameterIndex;

                if (parentInfo.Data is GenericTypeData parentGeneric)
                    reconstructedTypeName = parentGeneric.FullNameWithArgumentName(reconstructedTypeName, parameterIndex);
                else
                    return false;
                
                currentPlacement = parentInfo;
            }

            typeFullNameAsGeneric = reconstructedTypeName;
        }
        else
        {
            typeFullNameAsGeneric = parameterName;
            whereStatement = "where " + parameterName + " : " + targetType.FullName;
        }
        
        return true;
    }

    public TypePlacementInfo? GetPlacementInfoForType(TypeData targetType)
    {
        var root = new TypePlacementInfo(this, null);
        return SearchRecursive(targetType, root);
    }

    private TypePlacementInfo? SearchRecursive(TypeData targetType, TypePlacementInfo current)
    {
        if (Match(targetType))
        {
            return current;
        }
        
        if (this is GenericTypeData generic)
        {
            for (int i = 0; i < generic.Arguments.Count; i++)
            {
                var childData = generic.Arguments[i];

                var childInfo = new TypePlacementInfo(childData, current, i);
                
                var result = childData.SearchRecursive(targetType, childInfo);
                
                if (result != null)
                    return result;
            }
        }
        return null;
    }
}

public record UnexpectedTypeData() : TypeData("ERROR","ERROR",GenericConstraints.None);

public record SimpleTypeData(string Name, string Namespace, GenericConstraints SatisfiedGenericConstraints) : TypeData(Name, Namespace, SatisfiedGenericConstraints);



