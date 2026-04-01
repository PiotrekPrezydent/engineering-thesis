using LADR.SharedKernel.Domain.Models;

namespace LADR.Modules.Plugins.Domain;

public class Plugin : Entity
{
    public string Name { get; private set; }
    
    public List<Function> Functions { get; private set; }
    
    
}