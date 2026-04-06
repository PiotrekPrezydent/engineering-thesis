using LADR.Modules.Plugins.Domain.Models;
using LADR.Modules.Plugins.Domain.Repositories;

namespace LADR.Modules.Plugins.Infrastructure;

public class TestPluginInstanceRepository : IPluginInstanceRepository
{
    private List<PluginInstance> _instances;

    public TestPluginInstanceRepository()
    {
        _instances = new();
    }
    
    public PluginInstance GetById(Guid id)
    {
        return _instances.FirstOrDefault(x => x.Id == id);
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

    public bool Update(PluginInstance pluginInstance)
    {
        throw new NotImplementedException();
    }
}