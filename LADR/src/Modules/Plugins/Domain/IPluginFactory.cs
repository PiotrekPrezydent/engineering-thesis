using LADR.Modules.Plugins.Domain.Models;

namespace LADR.Modules.Plugins.Domain;

public interface IPluginFactory
{
    public Plugin CreateFromInstance(PluginInstance instance);
}