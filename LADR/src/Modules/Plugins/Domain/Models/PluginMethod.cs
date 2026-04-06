using LADR.SharedKernel.Domain.Models;

namespace LADR.Modules.Plugins.Domain.Models;

public class PluginMethod : ValueObject
{
    public string Name { get; set; }
    
    public string Description { get; set; }
    
    public IReadOnlyList<MethodParameter> Parameters { get; private set; }

    public PluginMethod(string name, string description, List<MethodParameter> parameters)
    {
        Name = name;
        Description = description;
        Parameters = parameters;
    }
}