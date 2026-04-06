using LADR.Modules.Plugins.Domain;
using LADR.Modules.Plugins.Domain.Models;

namespace LADR.Modules.Plugins.Infrastructure;

public class PluginServices : IPluginServices
{
    public object? InvokePluginMethod(Plugin plugin, PluginMethod method, params object[] parameters)
    {
        return plugin.PluginInstance.ParentType.GetMethod(method.Name).Invoke(plugin.PluginInstance.InstanceObject, parameters);
    }
}