using System.Security.Authentication.ExtendedProtection;
using LADR.Modules.Plugins.Application;
using LADR.Modules.Plugins.Domain;
using LADR.Modules.Plugins.Domain.Repositories;
using LADR.Modules.Plugins.Infrastructure;
using Microsoft.Extensions.DependencyInjection;

namespace LADR.Modules.Plugins.Tests.CLI;

public static class ModuleInitializer
{
    public static TestPluginCommands InitializeTestsCommands()
    {
        var services = new ServiceCollection();
        
        services.AddScoped<IPluginFactory, PluginFactory>();
        services.AddScoped<IPluginInstanceFactory, PluginInstanceFactory>();
        services.AddScoped<IPluginInstanceRepository, PluginInstanceRepository>();
        services.AddScoped<IPluginServices, PluginServices>();
        
        services.AddTransient<TestPluginCommands>();
        
        var serviceProvider = services.BuildServiceProvider();
        var testPluginCommands = serviceProvider.GetService<TestPluginCommands>();
        return testPluginCommands;
    }
}