using LADR.SharedKernel.Domain.Models;

namespace LADR.Modules.Plugins.Domain.Models;

public class PluginInstance : Entity, IAggregateRoot
{
    public object InstanceObject  { get; private set; }
    
    public Type ParentType { get; private set; }
    
    public PluginInstance(object instanceObject, Type parentType)
    {
        InstanceObject = instanceObject;
        ParentType = parentType;
    }
}