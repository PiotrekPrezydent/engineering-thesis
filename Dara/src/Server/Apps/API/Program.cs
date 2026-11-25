using System.Collections.Immutable;
using System.Reflection;
using System.Threading.Channels;
using Dara.Server.Apps.API.Authentication;
using Dara.Server.Apps.API.Hubs;
using Dara.Server.Apps.API.Processing;
using Dara.Server.Apps.API.Services;
using Dara.Server.Apps.API.Services.Seeding;
using Dara.Server.BuildingBlocks.Infrastructure.Common.Extensions;
using Dara.Server.BuildingBlocks.Infrastructure.Common.Types;
using Dara.Server.BuildingBlocks.Infrastructure.Configuration;
using Dara.Server.BuildingBlocks.Infrastructure.Messaging.EventBus;
using Dara.Server.BuildingBlocks.Integration;
using Dara.Server.Modules.Groups.Infrastructure;
using Dara.Server.Modules.Identity.Infrastructure;
using Dara.Server.Modules.Orchestration.Infrastructure;
using Dara.Server.Modules.Plugins.Infrastructure;
using Dara.Server.Modules.Profiles.Infrastructure;
using Dara.Shared.Common.Configuration;
using Dara.Shared.Contracts;
using Microsoft.AspNetCore.SignalR;

namespace Dara.Server.Apps.API;

public class Program
{
    public static async Task Main(string[] args)
    {
        // values of .env become environment variables, so the configuration below reads them
        DotEnv.Load(Path.Combine(AppContext.BaseDirectory, ".env"));

        var modulesRoots = new IModuleCompositionRoot[]
        {
            new IdentityCompositionRoot(),
            new GroupsCompositionRoot(),
            new ProfilesCompositionRoot(),
            new PluginsCompositionRoot(),
            new OrchestrationCompositionRoot()
        };

        var app = CreateApplication(args, modulesRoots);
        app.Run();
    }

    // modules build their containers before the host, so tests pass their own roots here (e.g. a scripted model provider)
    public static WebApplication CreateApplication(string[] args, IReadOnlyList<IModuleCompositionRoot> modulesRoots)
    {
        var builder = WebApplication.CreateBuilder(args);
        
        builder.Services.AddMemoryCache();
        
        builder.Services.AddSignalR(options => 
        { 
            options.AddFilter<AppHubFilter>();
        });
        
        builder.Services.AddAuthentication(options =>
        {
            options.AddScheme<ClientIdentifierAuthHandler>(nameof(ClientIdentifierAuthHandler),null);
        });
        
        // one bus per host, so several hosts in one process (tests) do not share events
        var eventBus = new InMemoryEventBus();
        
        AddNotificationHandlersInAssembly(builder.Services, Assembly.GetExecutingAssembly(), eventBus);

        builder.Services.AddScoped<LogModulesDataService>();

        builder.Services.AddSingleton<SeedEnvironment>();
        builder.Services.AddScoped<ISeeder, IdentitySeeder>();
        builder.Services.AddScoped<ISeeder, ProfilesSeeder>();
        builder.Services.AddScoped<ISeeder, GroupsSeeder>();
        builder.Services.AddScoped<ISeeder, PluginsSeeder>();
        builder.Services.AddScoped<ISeeder, OrchestrationSeeder>();
        builder.Services.AddHostedService<StartupSeedService>();

        builder.Services.AddHostedService<HubNotificationsProcessor>();

        foreach (var module in modulesRoots)
            module.Initialize(builder.Services, eventBus);
        
        var app = builder.Build();
        
        app.UseAuthentication();
        app.UseAuthorization();
        // await app.StartAsync();
        // await Task.Delay(TimeSpan.FromSeconds(5));
        // foreach (var modulesRoot in modulesRoots)
        // {
        //    var cast = (ModuleCompositionRootBase)modulesRoot;
        //    cast.LogBenchmark();
        //    
        // }
        // await app.StopAsync();
        
        app.MapHub<AppHub>(Connections.HubName);
        return app;
    }

    static void AddNotificationHandlersInAssembly(IServiceCollection services, Assembly assembly, IEventBus eventBus)
    {
        var channel = Channel.CreateUnbounded<IIntegrationEvent>();
        services.AddSingleton(channel);
        services.AddSingleton(eventBus);
        
        var handlers = assembly.GetImplementationsOfOpenGeneric(typeof(IHubNotificationHandler<>));
        
        foreach (var handler in handlers)
        {
            var eventType = handler.Interface.GenericTypeArguments[0];
            var eventTypeKey = Activator.CreateInstance(typeof(TypeKey<>).MakeGenericType(eventType)) as ITypeKey<IIntegrationEvent>;
            
            services.AddScoped(handler.Interface, handler.Implementation);
            
            eventTypeKey!.ExecuteGenericAction(new ChannelWriterIntegrationEventRegistrator(channel, eventBus));
        }
    }
    
    class ChannelWriterIntegrationEventRegistrator(Channel<IIntegrationEvent> channel, IEventBus eventBus) : ITypeKeyAction<IIntegrationEvent>
    {
        public void Execute<TType>(ITypeKey<IIntegrationEvent> typeKey) where TType : IIntegrationEvent
        {
            var handler = new ChannelWriterIntegrationEventHandler<TType>(channel);
            eventBus.Subscribe(handler);
        }
    }
}
