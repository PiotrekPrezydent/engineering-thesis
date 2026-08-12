namespace Dara.Shared.Testing.Plugins;

public static class FunctionSamples
{
    public static FunctionData SampleA = new()
    {
        Name = "MyTestFunction", 
        Description = "",
        Parameters = new List<ParameterData>
        {
            new()
            {
                Name = "a",
                Type = "number",
                IsRequired = true
            },
            new()
            {
                Name = "b",
                Type = "number",
                IsNullable = true,
                DefaultValue = 1
            },
            
            new()
            {
                Name = "tags",
                Type = "array",
                IsRequired = false,
                Items = new ParameterData
                { 
                    Type = "string"
                }
            },
            
            new()
            {
                Name = "metadata",
                Type = "object",
                IsRequired = false,
                Properties = new List<ParameterData>
                {
                    new() { Name = "id", Type = "integer" },
                    new() { Name = "isActive", Type = "boolean" }
                }
            }
        }
    };
}