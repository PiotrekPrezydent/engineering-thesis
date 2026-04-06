using LADR.SharedKernel.Domain.Models;

namespace LADR.Modules.Plugins.Domain.Models;

//TD add adding and removing method validation
public class Plugin : IAggregate
{
    public string Name { get; private set; } // Name of plugin
    
    public string Description { get; private set; } // Description of plugin
    
    public List<PluginMethod> Methods { get; private set; } // all plugin methods
    
    public PluginInstance PluginInstance { get; private set; } // instance of plugin 

    public Plugin(string name, string description)
    {
        Name = name;
        Description = description;
        Methods = new();
        PluginInstance = null;
    }

    public void AddInstance(PluginInstance instance)
    {
        if (PluginInstance == null)
            PluginInstance = instance;
    }

    public void AddMethod(PluginMethod method)
    {
        Methods.Add(method);
    }

    public void AddMethodRange(List<PluginMethod> methods)
    {
        Methods.AddRange(methods);
    }

    public void RemoveMethod(PluginMethod method)
    {
        Methods.Remove(method);
    }
    
}