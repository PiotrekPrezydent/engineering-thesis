
using LADR.Modules.Plugins.Domain.Models;

namespace LADR.Modules.Plugins.Domain;

public interface IPluginInstanceFactory
{
    public PluginInstance CreateFromType(Type exportedPluginType);

    public PluginInstance CreateFromPath(string path);
    
    public PluginInstance CreateFromJson(string json);
}