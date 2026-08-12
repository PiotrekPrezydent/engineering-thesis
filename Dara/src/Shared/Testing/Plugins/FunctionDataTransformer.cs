using System.Text.Json;
using System.Text.Json.Nodes;
using Microsoft.Extensions.AI;

namespace Dara.Shared.Testing.Plugins;

public static class FunctionDataTransformer
{
    public static AIFunctionDeclaration Transform(FunctionData function)
    {
        var name = function.Name;
        var description = function.Description;
        var properties = new JsonObject();
        var required = new JsonArray();

        foreach (var param in function.Parameters)
        {
            properties[param.Name] = GenerateSchemaForParameter(param);
            
            if (param.IsRequired && !param.HasDefaultValue)
            {
                required.Add(param.Name);
            }
        }

        var root = new JsonObject
        {
            ["type"] = "object",
            ["properties"] = properties
        };

        if (required.Count > 0)
        {
            root["required"] = required;
        }
        var schema = JsonSerializer.SerializeToElement(root);
        if(string.IsNullOrEmpty(function.ReturnType))
            return AIFunctionFactory.CreateDeclaration(name, description, schema);

        var returnObject = new JsonObject
        {
            ["type"] = function.ReturnType
        };
   

        return AIFunctionFactory.CreateDeclaration(name, description, schema,JsonSerializer.SerializeToElement(returnObject));
    }
    
    private static JsonObject GenerateSchemaForParameter(ParameterData param)
    {
        var schema = new JsonObject();
        
        if (param.IsNullable)
        {
            schema["type"] = new JsonArray { param.Type, "null" };
        }
        else
        {
            schema["type"] = param.Type;
        }
        
        if (!string.IsNullOrWhiteSpace(param.Description))
        {
            schema["description"] = param.Description;
        }
        
        if (param.HasDefaultValue)
        {
            schema["default"] = JsonSerializer.SerializeToNode(param.DefaultValue);
        }
        
        if (param.EnumValues != null && param.EnumValues.Count > 0)
        {
            var enumArray = new JsonArray();
            foreach (var val in param.EnumValues) enumArray.Add(val);
            schema["enum"] = enumArray;
        }
        
        if (param.Type == "array" && param.Items != null)
        {
            schema["items"] = GenerateSchemaForParameter(param.Items);
        }
        
        if (param.Type == "object" && param.Properties != null)
        {
            var objProps = new JsonObject();
            var objReq = new JsonArray();

            foreach (var prop in param.Properties)
            {
                objProps[prop.Name] = GenerateSchemaForParameter(prop);
                
                if (prop.IsRequired && !prop.HasDefaultValue)
                {
                    objReq.Add(prop.Name);
                }
            }

            schema["properties"] = objProps;
            if (objReq.Count > 0) schema["required"] = objReq;
        }

        return schema;
    }
}