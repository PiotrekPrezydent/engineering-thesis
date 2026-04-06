using LADR.SharedKernel.Domain.Models;

namespace LADR.Modules.Plugins.Domain.Models;

public class PluginMethod : ValueObject
{
    public string Name { get; private set; } // name of method
    
    public string Description { get; private set; } // description of method
    
    public IReadOnlyList<MethodParameter> Parameters { get; private set; } // Method Parameters

    public PluginMethod(string name, string description, List<MethodParameter> parameters)
    {
        Name = name;
        Description = description;
        Parameters = parameters;
    }
}