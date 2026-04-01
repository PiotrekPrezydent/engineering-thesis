using LADR.SharedKernel.Domain.Models;

namespace LADR.Modules.Plugins.Domain;

public class Function : Entity
{
    public string Name { get; private set; }
    
    public string Description { get; private set; }
    
    public Type ReturnType { get; private set; }
    
    public List<Parameter> Parameters { get; private set; }
}