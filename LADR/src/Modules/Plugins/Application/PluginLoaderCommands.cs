using LADR.Modules.Plugins.Domain;
using LADR.Modules.Plugins.Domain.Repositories;

namespace LADR.Modules.Plugins.Application;

//TD add more commands
public class PluginLoaderCommands
{
    readonly IPluginInstanceFactory _pluginInstanceFactory;
    readonly IPluginInstanceRepository _repository;
    readonly IPluginFactory _pluginFactory;

    public PluginLoaderCommands(IPluginInstanceFactory pluginInstanceFactory, IPluginInstanceRepository repository, IPluginFactory pluginFactory)
    {
        _pluginInstanceFactory = pluginInstanceFactory;
        _repository = repository;
        _pluginFactory = pluginFactory;
    }

    public string[] GetAllAvaiblePluginsPath(string directory)
    {
        return Directory.GetFiles(directory,"*.dll");
    }

    public string PreviewPluginDetails(string pluginPath)
    {
        return "";
    }

    public void LoadPlugin(string pluginPath)
    {
        var instance = _pluginInstanceFactory.CreateFromPath(pluginPath);
        _pluginFactory.CreateFromPluginInstance(instance);
        
        _repository.Add(instance);
    }

    public void RefreshPlugins()
    {
        
    }
}