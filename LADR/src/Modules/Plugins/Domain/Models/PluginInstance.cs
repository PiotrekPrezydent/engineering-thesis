using LADR.SharedKernel.Domain.Models;

namespace LADR.Modules.Plugins.Domain.Models;

public class PluginInstance : Entity, IAggregateRoot
{
    public object InstanceObject  { get; private set; } // object of this plugin
    
    public Type ParentType { get; private set; } // type exported from this plugin
    
    public Plugin Plugin { get; private set; }
    
    public PluginInstance(object instanceObject, Type parentType)
    {
        InstanceObject = instanceObject;
        ParentType = parentType;
    }

    public void AddPlugin(Plugin plugin)
    {
        Plugin = plugin;
    }
}