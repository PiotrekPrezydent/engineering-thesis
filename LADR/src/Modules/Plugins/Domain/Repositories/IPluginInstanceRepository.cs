using LADR.Modules.Plugins.Domain.Models;

namespace LADR.Modules.Plugins.Domain.Repositories;

public interface IPluginInstanceRepository
{
    public PluginInstance GetById(Guid id);
    
    public IEnumerable<PluginInstance> GetAll();
    
    public bool Add(PluginInstance pluginInstance);
}