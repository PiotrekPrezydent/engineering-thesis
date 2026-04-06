using LADR.Modules.Plugins.Domain;
using LADR.Modules.Plugins.Domain.Models;

namespace LADR.Modules.Plugins.Infrastructure;

public class TestPluginInstanceFactory : IPluginInstanceFactory
{
    public PluginInstance Create(object instanceObj)
    {
        return new PluginInstance(instanceObj, instanceObj.GetType());
    }
}