using LADR.SharedKernel.Domain.Models;

namespace LADR.Modules.Plugins.Domain.Models;

public class MethodParameter : ValueObject
{
    public string Name { get; private set; } // Name of method parameter
    
    public string Description { get; private set; } // Description of method parameter
    
    public Type ParameterType { get; private set; } // Type of method parameter
    
    public MethodParameter(string name, string description, Type parameterType)
    {
        Name = name;
        Description = description;
        ParameterType = parameterType;
    }
}