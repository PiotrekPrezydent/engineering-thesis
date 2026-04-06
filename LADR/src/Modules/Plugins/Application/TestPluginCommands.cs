using LADR.Modules.Plugins.Domain;
using LADR.Modules.Plugins.Domain.Repositories;

namespace LADR.Modules.Plugins.Application;

public class TestPluginCommands
{
    private readonly IPluginInstanceRepository _repository;

    private readonly IPluginFactory _pluginFactory;

    private readonly IPluginInstanceFactory _pluginInstanceFactory;
    
    private readonly IPluginServices _pluginServices;

    public TestPluginCommands(IPluginInstanceRepository repository, IPluginFactory pluginFactory,
        IPluginInstanceFactory pluginInstanceFactory, IPluginServices pluginServices)
    {
        _repository = repository;
        _pluginFactory = pluginFactory;
        _pluginInstanceFactory = pluginInstanceFactory;
        _pluginServices = pluginServices;
    }

    public void AddPlugin(string path)
    {
        if (_repository == null)
            return;

        var pi = _pluginInstanceFactory.CreateFromPath(path);
        _repository.Add(pi);
    }

    public void CreatePlugins()
    {
        var all = _repository.GetAll();
        foreach (var plugin in all)
        {
            _pluginFactory.CreateFromPluginInstance(plugin);
        }
    }

    public void InvokePluginMethod(string pluginName, string methodName, params object[] parameters)
    {
        var plugin = _repository.GetAll().First(e=>e.Plugin.Name == pluginName).Plugin;
        var method = plugin.Methods.First(m => m.Name == methodName);
        
        _pluginServices.InvokePluginMethod(plugin, method, parameters);
    }
}