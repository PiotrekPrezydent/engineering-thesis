using LADR.Modules.Plugins.Domain.Models;

namespace LADR.Modules.Plugins.Domain;

public interface IPluginServices
{
    public object? InvokePluginMethod(Plugin plugin, PluginMethod method, params object[] parameters);
}