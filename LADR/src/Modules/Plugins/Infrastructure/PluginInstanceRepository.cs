using LADR.Modules.Plugins.Domain.Exceptions;
using LADR.Modules.Plugins.Domain.Models;
using LADR.Modules.Plugins.Domain.Repositories;

namespace LADR.Modules.Plugins.Infrastructure;

public class PluginInstanceRepository : IPluginInstanceRepository
{
    private List<PluginInstance> _instances = new();
    
    public PluginInstance GetById(Guid id)
    {
        foreach (var pluginInstance in _instances)
            if(pluginInstance.Id == id)
                return pluginInstance;

        throw new PluginInstanceNotFoundException();
    }

    public IEnumerable<PluginInstance> GetAll()
    {
        return _instances;
    }

    public bool Add(PluginInstance pluginInstance)
    {
        _instances.Add(pluginInstance);
        return true;
    }
}