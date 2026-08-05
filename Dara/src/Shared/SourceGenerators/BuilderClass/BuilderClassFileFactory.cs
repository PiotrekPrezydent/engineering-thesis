using System.Collections.Generic;
using System.Linq;
using System.Text;
using Dara.Shared.SourceGenerators.BuilderClass.Models;
using Dara.Shared.SourceGenerators.BuilderCollections;
using Dara.Shared.SourceGenerators.Common;
using Dara.Shared.SourceGenerators.Extensions;

namespace Dara.Shared.SourceGenerators.BuilderClass;

public static class BuilderClassFileFactory
{
    public static FileDeclaration GenerateBuilderClassFileDeclaration(BuilderClassSpecification specification)
    {
        var methods = CollectPropertiesMethods(specification.Properties.ToArray());
        
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
                        }
                        
                        public {{specification.ClassName}} Build()
                        {
                            return {{specification.ClassInstanceFieldName}};
                        }
                        
                        {{CreateBuilderClassMethodsCode(methods,specification.ClassInstanceFieldName,specification.BuilderClassName).AppendIntendOnNewLine(3)}}
                    }
                }
            }
            """";
        
        return new FileDeclaration(specification.ClassName, text);
    }
    

    static List<BuilderClassMethodData> CollectPropertiesMethods(PropertyData[] properties)
    {
        var result = new List<BuilderClassMethodData>();
        foreach (var property in properties)
        {
            var methodName = "With"+property.Name;
            var parameterType = property.Type.FullName;
            var parameterName = property.NameAsParameter;
            var fieldName = property.Name;
            bool isTypeObsolete = false;
            BuilderClassMethodData primaryMethodData = null;
            
            foreach (var attribute in property.Attributes)
            {
                if (attribute is ObsoleteMethodOnRepeatedTypeAttributeData obsoleteAttribute)
                {
                    var dataToReplace = obsoleteAttribute.NonRepeatableTypeData;
                    var genericParameterName = "T" + dataToReplace.Name;
                    if (property.Type.TryReplaceDataAsGenericType(dataToReplace, genericParameterName,
                            out string genericTypeName, out var whereStatement))
                    {

                        primaryMethodData = new GenericBuilderClassMethodData(methodName, genericTypeName,
                            parameterName,fieldName, $"<{genericParameterName}>", whereStatement);
                        isTypeObsolete = true;
                        
                        result.Add(new SimpleBuilderClassMethodData(methodName, parameterType, parameterName,fieldName,$"[Obsolete(\"Type: {dataToReplace.Name} cannot be repeated\",true)]"));
                    }
                }
            }

            if (property.Type is GenericTypeData genericType)
            {
                if (genericType.IsCollection)
                {
                    var arg = genericType.Arguments[0];
                    // primaryMethodData = new CollectionBuilderClassMethodData(methodName, parameterType, parameterName,
                    //     fieldName, arg.FullName);
                    result.Add(new CollectionBuilderClassMethodData(methodName, parameterType, parameterName,
                        fieldName, arg.FullName,isTypeObsolete));
                }
            }
       
            
            if(primaryMethodData == null)
                primaryMethodData = new SimpleBuilderClassMethodData(methodName, parameterType, parameterName,fieldName);
            
            result.Add(primaryMethodData);
        }
        return  result;
    }
    
    static StringBuilder CreateBuilderClassMethodsCode(List<BuilderClassMethodData> methods, string classInstanceFieldName, string builderClassName)
    {
        StringBuilder sb = new();
        foreach (var method in methods)
        {
            if (method.AttributeText != "")
                sb.AppendLine(method.AttributeText);
            
            if(method is SimpleBuilderClassMethodData simpleMethod)
                sb.AppendLine(CreateSimpleMethodCode(builderClassName, classInstanceFieldName, simpleMethod));
            
            if(method is GenericBuilderClassMethodData genericMethod)
                sb.AppendLine(CreateGenericMethodCode(builderClassName, classInstanceFieldName, genericMethod));
            
            if(method is CollectionBuilderClassMethodData collectionMethod)
                sb.AppendLine(CreateBuilderCollectionMethodCode(builderClassName, classInstanceFieldName, collectionMethod));

            sb.AppendLine();
        }
    
        return sb;
    }

     static string CreateSimpleMethodCode(string builderClassName, string classInstanceFieldName, SimpleBuilderClassMethodData data)
     {
         var text = 
             $$""""
             public {{builderClassName}} {{data.MethodName}}({{data.ParameterType}} {{data.ParameterName}})
             {
                 {{classInstanceFieldName}}.{{data.FieldName}} = {{data.ParameterName}};
                 return this;
             }
             """";
         return text;
     }

     static string CreateGenericMethodCode(string builderClassName, string classInstanceFieldName,
         GenericBuilderClassMethodData data)
     {
         var text = 
             $$""""
               public {{builderClassName}} {{data.MethodName}}{{data.GenericParametersStatement}}({{data.ParameterType}} {{data.ParameterName}}) {{data.WhereStatement}}
               {
                   {{classInstanceFieldName}}.{{data.FieldName}} = {{data.ParameterName}};
                   return this;
               }
               """";
         return text;
     }

     static string CreateBuilderCollectionMethodCode(string builderClassName, string classInstanceFieldName,
         CollectionBuilderClassMethodData data)
     {
         var colType = BuilderCollectionFileFactory.GetFormatableName(data.isTypeObsolete);
         var formated = string.Format(colType, data.CollectionParameterName);
         var text = 
             $$""""
               public {{builderClassName}} {{data.MethodName}}(Action<{{formated}}> configure)
               {
                   {{classInstanceFieldName}}.{{data.FieldName}} = new List<{{data.CollectionParameterName}}>();
                   var builderCollection = new {{formated}}({{classInstanceFieldName}}.{{data.FieldName}}.ToList());
                   configure(builderCollection);
                   
                   return this;
               }
               """";
         return text;
     }
}