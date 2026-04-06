
using LADR.Modules.Plugins.Domain.Models;

namespace LADR.Modules.Plugins.Domain;

public interface IPluginInstanceFactory
{
    public PluginInstance Create(object instanceObj);

}