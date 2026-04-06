using LADR.SharedKernel.Domain.Models;

namespace LADR.Modules.Plugins.Domain.Models;

public class Plugin : IAggregate
{
    public string Name { get; private set; }
    
    public string Description { get; private set; }
    
    public List<PluginMethod> Methods { get; private set; }
    
    public PluginInstance PluginInstance { get; private set; }

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

    public string AllInfo()
    {
        string ret = "";
        ret += Name + "\n";
        ret += Description + "\n";
        ret += "\nMETHODS:\n";
        foreach (var method in Methods)
        {
            ret += method.Name + " - " + method.Description + "\n";
        }

        ret += "\n";
        ret += PluginInstance.ParentType.Name + " --- " + PluginInstance.InstanceObject.ToString();
        ret += "\n\n\n";

        return ret;
    }
}