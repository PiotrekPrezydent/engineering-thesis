using LADR.SharedKernel.Domain.Models;

namespace LADR.Modules.Plugins.Domain.Models;

public class MethodParameter : ValueObject
{
    public string Name { get; private set; }
    
    public string Description { get; private set; }
    
    public Type ParameterType { get; private set; }
    
    public MethodParameter(string name, string description, Type parameterType)
    {
        Name = name;
        Description = description;
        ParameterType = parameterType;
    }
}