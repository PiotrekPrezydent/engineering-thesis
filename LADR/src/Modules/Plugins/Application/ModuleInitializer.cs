using LADR.Modules.Plugins.Domain;
using LADR.Modules.Plugins.Domain.Repositories;
using LADR.Modules.Plugins.Infrastructure;
using Microsoft.Extensions.DependencyInjection;

namespace LADR.Modules.Plugins.Application;

public static class ModuleInitializer
{
    private static ServiceProvider _services;

    public static void Initialize()
    {
        var services = new ServiceCollection();
        services = new ServiceCollection();
        
        services.AddScoped<IPluginFactory, PluginFactory>();
        services.AddScoped<IPluginInstanceFactory, PluginInstanceFactory>();
        services.AddScoped<IPluginInstanceRepository, PluginInstanceRepository>();
        services.AddScoped<IPluginServices, PluginServices>();

        services.AddTransient<PluginLoaderCommands>();
        services.AddTransient<PluginMethodInvokeCommand>();
        
        _services = services.BuildServiceProvider();
    }
    
    public static PluginLoaderCommands GetPluginLoaderCommands() =>  _services.GetService<PluginLoaderCommands>();
    
    public static PluginMethodInvokeCommand GetPluginMethodCommands() => _services.GetService<PluginMethodInvokeCommand>();
    
    
}