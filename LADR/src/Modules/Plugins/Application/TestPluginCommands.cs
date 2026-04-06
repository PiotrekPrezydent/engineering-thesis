using LADR.Modules.Plugins.Domain;
using LADR.Modules.Plugins.Domain.Repositories;

namespace LADR.Modules.Plugins.Application;

public class TestPluginCommands
{
    private readonly IPluginInstanceRepository _repository;

    private readonly IPluginFactory _pluginFactory;
    
    private readonly IPluginInstanceFactory _pluginInstanceFactory;

    public TestPluginCommands(IPluginInstanceRepository repository, IPluginFactory pluginFactory, IPluginInstanceFactory pluginInstanceFactory)
    {
        _repository = repository;
        _pluginFactory = pluginFactory;
        _pluginInstanceFactory = pluginInstanceFactory;
    }

    public void PrintAllPlugins()
    {
        var all = _repository.GetAll();
        Console.WriteLine("COUNT: " + all.Count());
        foreach (var pluginObject in all)
        {
            var p = _pluginFactory.CreateFromInstance(pluginObject);
            Console.WriteLine(p.AllInfo());
        }
    }

    public void AddPlugin(object pluginObject)
    {
        if (_repository == null)
        {
            Console.WriteLine("NULLREPO");
            return;
        }
       var pi = _pluginInstanceFactory.Create(pluginObject);
       _repository.Add(pi);
    }
}