using LADR.SharedKernel.Domain.Models;

namespace LADR.Modules.Plugins.Domain;

public class Parameter : Entity
{
    public string Name { get; private set; }
    
    public string Description { get; private set; }
    
    public Type Type { get; private set; }
}