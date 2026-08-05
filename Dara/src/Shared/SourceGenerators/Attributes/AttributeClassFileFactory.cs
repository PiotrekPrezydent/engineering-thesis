using System;
using System.Linq;
using Dara.Shared.SourceGenerators.Common;
using Dara.Shared.SourceGenerators.Extensions;

namespace Dara.Shared.SourceGenerators.Attributes;

public static class AttributeClassFileFactory
{
    public static FileDeclaration CreateAttributeFileDeclaration(AttributeClassSpecification classSpecification)
    {
        var text = 
            $$""""
            namespace {{classSpecification.Namespace}}
            {
                {{CreateAttributeUsageCode(classSpecification.UsageData)}}
                public class {{classSpecification.ClassName}} : System.Attribute
                {
                    public {{classSpecification.ClassName}}({{CreateAttributeParametersCode(classSpecification.ParametersDatas)}})
                    {
                    }
                }   
            }   
            """";
          return new(classSpecification.ClassName, text);
    }
    
    static string CreateAttributeUsageCode(AttributeUsageData data)
    {
        string allowMultiple = $"AllowMultiple = {data.AllowMultiple.ToString().ToLowerInvariant()}";
        string inherited = $"Inherited = {data.Inherited.ToString().ToLowerInvariant()}";
        string targets = string.Join(" | ", data.Targets.FlagsToArray().Select(e => e.GetType() + "." + e));

        return $"[{typeof(AttributeUsageAttribute).FullName}({targets}, {allowMultiple},  {inherited})]";
    }

    static string CreateAttributeParametersCode(AttributeParameterData[] datas)
    {
        Array.Sort(datas, (x,y) => x.IsOptional.CompareTo(y.IsOptional));
        
        return string.Join(", ", datas.Select(AttributeParameterDataToText));
    }

    static string AttributeParameterDataToText(AttributeParameterData data)
    {
        string parameterText = $"{data.ParameterType.FullName} {data.ParameterName}";
        string parameterValueText = "";
        
        if (data.IsOptional)
        {
            parameterValueText += " = ";
            
            if (data.DefaultValue is null)
                parameterValueText += "null";
            else if(data.DefaultValue is string)
                parameterValueText += $"\"{data.DefaultValue}\"";
            else if(data.DefaultValue is bool)
                parameterValueText += data.DefaultValue.ToString().ToLowerInvariant();
            else
                parameterValueText += data.DefaultValue.ToString();
        }
        
        return parameterText + parameterValueText;
    }
}