using System.Collections.Generic;
using System.Linq;
using System.Text;
using Dara.Shared.SourceGenerators.BuilderClass.Methods;
using Dara.Shared.SourceGenerators.BuilderClass.Models;
using Dara.Shared.SourceGenerators.BuilderCollections;
using Dara.Shared.SourceGenerators.Common;
using Dara.Shared.SourceGenerators.Extensions;

namespace Dara.Shared.SourceGenerators.BuilderClass;

public static class BuilderClassFileFactory
{
    public static FileDeclaration GenerateBuilderClassFileDeclaration(BuilderClassSpecification specification)
    {
        var factory = new BuilderMethodFactory(specification.BuilderClassName,specification.ClassInstanceFieldName);
        var text =
            $$""""
            namespace {{specification.Namespace}}
            {
                public partial class {{specification.ClassName}}
                {
                    private {{specification.ClassName}}() { }
                    
                    public static {{specification.BuilderClassName}} GetBuilder()
                    {
                        return new {{specification.BuilderClassName}}();
                    }
                    
                    public class {{specification.BuilderClassName}}
                    {
                        private readonly {{specification.ClassName}} {{specification.ClassInstanceFieldName}};
                        
                        public {{specification.BuilderClassName}}()
                        {
                            {{specification.ClassInstanceFieldName}} = new();
                            {{CreateCollectionPropertiesInitializationCode(specification.Properties.ToArray(),specification.ClassInstanceFieldName).AppendIntendOnNewLine(4).RemoveLastNewLine()}}
                        }
                        
                        public {{specification.ClassName}} Build()
                        {
                            return {{specification.ClassInstanceFieldName}};
                        }
                        
                        {{CreateBuilderClassMethodsCode(specification.Properties.ToArray(), factory).AppendIntendOnNewLine(3)}}
                    }
                }
            }
            """";
        
        return new FileDeclaration(specification.ClassName, text);
    }

    static StringBuilder CreateCollectionPropertiesInitializationCode(PropertyData[] properties, string classInstanceFieldName)
    {
        var sb = new StringBuilder();
        foreach (var property in properties)
        {
            if (property.Type is GenericTypeData generic && generic.IsCollection)
            {
                sb.AppendLine(
                    $"{classInstanceFieldName}.{property.Name} = new List<{generic.Arguments[0].FullName}>();");
            }
        }

        return sb;
    }

    static StringBuilder CreateBuilderClassMethodsCode(PropertyData[] properties, BuilderMethodFactory factory)
    {
        StringBuilder sb = new();
        foreach (var property in properties)
        {
            var methods =GetPropertyMethods(property);
            foreach (var builderMethodData  in methods)
            {
                if(builderMethodData is BaseMethodData baseMethodData && baseMethodData.AddObsoleteAttribute)
                    sb.AppendLine($"[Obsolete(\"\",true)]");
                
                sb.AppendLine(factory.GetMethodCodeByType(builderMethodData));
                sb.AppendLine();
            }
        }

        return sb;
    }

    static List<IBuilderMethodData> GetPropertyMethods(PropertyData property)
    {
        var result = new List<IBuilderMethodData>();

        CustomMethodNameAttributeData customMethodName = null;
        ObsoleteMethodOnRepeatedTypeAttributeData obsoleteMethod = null;
        foreach (var attr in property.Attributes)
        {
            if (attr is CustomMethodNameAttributeData specialName)
                customMethodName = specialName;
            
            if(attr is ObsoleteMethodOnRepeatedTypeAttributeData obsolete)
                obsoleteMethod = obsolete;
        }

        string methodName = "";
        
        if (customMethodName != null)
            methodName = customMethodName.MethodName;
        else
            methodName = $"With{property.Name}";

        if (property.Type is GenericTypeData generic && generic.IsCollection)
        {
            string collectionArgument = generic.Arguments[0].FullName;
            string collectionType = "";
            methodName = $"Configure{property.Name}";
            
            string format = BuilderCollectionNames.GetFormatableText((obsoleteMethod == null) ? BuilderCollectionNames.ClassName : BuilderCollectionNames.TypeIgnoringClassName);
            collectionType = string.Format(format, collectionArgument);
            var baseData = new BaseMethodData(methodName, property.Name, collectionType, "configure");
            
            result.Add(new CollectionMethodData(collectionArgument, baseData));
        }
        else
        {
            bool addObsolete = false;
            if (obsoleteMethod != null)
            {
                var parameterName = "T" + obsoleteMethod.NonRepeatableTypeData.Name;
                property.Type.TryReplaceDataAsGenericType(obsoleteMethod.NonRepeatableTypeData, parameterName, out string typeGenericName, out string whereStatement);
                var baseData = new BaseMethodData(methodName,  property.Name, typeGenericName, property.NameAsParameter);
                
                result.Add(new GenericMethodData($"<{parameterName}>",whereStatement, baseData));
                addObsolete = true;
            }   
            result.Add(new BaseMethodData(methodName, property.Name, property.Type.FullName, property.NameAsParameter, addObsolete));
        }
        
        return result;
    } 
}