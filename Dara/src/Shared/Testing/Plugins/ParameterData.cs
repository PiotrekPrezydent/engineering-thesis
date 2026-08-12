namespace Dara.Shared.Testing.Plugins;

public class ParameterData
{
    public string Name { get; set; }
    
    public string Type { get; set; } 
    public string Description { get; set; }
    
    public bool IsRequired { get; set; } = true;
    
    public bool IsNullable { get; set; } 
    
    public bool HasDefaultValue => DefaultValue != null;
    public object? DefaultValue { get; set; }
    
    public ParameterData Items { get; set; }
    
    public List<ParameterData> Properties { get; set; }
    
    public List<string> EnumValues { get; set; }
}