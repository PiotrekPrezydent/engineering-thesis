using System.Reflection;
using Dara.Server.Apps.API.Common.Authentication;
using Dara.Server.Apps.API.Common.Authorization;
using Dara.Server.Apps.API.Common.Endpoints;
using Dara.Server.Apps.API.Common.FunctionCalls;
using Dara.Server.Apps.API.Common.Diagnostics;
using Dara.Server.Apps.API.Common.Hubs;
using Dara.Server.Apps.API.Common.Notifications;
using Dara.Server.Apps.API.Common.Presence;
using Dara.Server.Apps.API.Common.Seeding;
using Dara.Server.Apps.API.Modules.Authorization;
using Dara.Server.Apps.API.Modules.Groups;
using Dara.Server.Apps.API.Modules.Identity;
using Dara.Server.Apps.API.Modules.Orchestration;
using Dara.Server.Apps.API.Modules.Plugins;
using Dara.Server.BuildingBlocks.Infrastructure.Configuration;
using Dara.Server.BuildingBlocks.Infrastructure.Messaging.EventBus;
using Dara.Server.Modules.Authorization.Infrastructure;
using Dara.Server.Modules.Groups.Infrastructure;
using Dara.Server.Modules.Identity.Infrastructure;
using Dara.Server.Modules.Orchestration.Infrastructure;
using Dara.Server.Modules.Plugins.Infrastructure;
using Dara.Shared.Common.Configuration;
using Dara.Shared.Contracts;
using Microsoft.AspNetCore.SignalR;

namespace Dara.Server.Apps.API;

// the host: Common/ holds what every module shares (authentication, the hub, hub notifications, the seed), each
// folder in Modules/ what the host adds for one module (http endpoints, hub notifications, seeder, mapping to
// the contract) behind its Add<Module>Api and Map<Module>Api
public class Program
{
    public static async Task Main(string[] args)
    {
        // values of .env become environment variables, so the configuration below reads them
        DotEnv.Load(Path.Combine(AppContext.BaseDirectory, ".env"));

        var modulesRoots = new IModuleCompositionRoot[]
        {
            new IdentityCompositionRoot(),
            new AuthorizationCompositionRoot(),
            new GroupsCompositionRoot(),
            new PluginsCompositionRoot(),
            new OrchestrationCompositionRoot()
        };

        var app = CreateApplication(args, modulesRoots);
        app.Run();
    }

    // modules build their containers before the host, so tests pass their own roots here (e.g. a scripted model provider);
    // mapExtra maps endpoints of a test host (e.g. probes of the authorization policies); configureServices runs after the
    // registrations of the host, so a test replaces a service (e.g. the port of function call results)
    public static WebApplication CreateApplication(string[] args, IReadOnlyList<IModuleCompositionRoot> modulesRoots, Action<WebApplication>? mapExtra = null,
        Action<IServiceCollection>? configureServices = null)
    {
        var builder = WebApplication.CreateBuilder(args);

        var timing = SignalRTimingOptions.Read(builder.Configuration);
        builder.Services.AddSignalR(options =>
        {
            options.KeepAliveInterval = timing.KeepAliveInterval;
            options.ClientTimeoutInterval = timing.ClientTimeoutInterval;
            // the app hub has no methods, the filter checks marked methods of any other hub (tests)
            options.AddFilter<AuthorizationHubFilter>();
        });
        builder.Services.AddSingleton<ClientPresenceTracker>();
        builder.Services.AddSingleton<IClientPresence>(sp => sp.GetRequiredService<ClientPresenceTracker>());

        builder.AddDaraAuthentication();
        // minimal api checks the token only on endpoints that bind a form (the panel, TASK-ENG-036);
        // json endpoints of the panel are covered by the SameSite=Strict cookie
        builder.Services.AddAntiforgery();
        builder.Services.AddProblemDetails();

        // one bus per host, so several hosts in one process (tests) do not share events
        var eventBus = new InMemoryEventBus();
        builder.Services.AddHubNotifications(Assembly.GetExecutingAssembly(), eventBus);

        builder.Services.AddScoped<LogModulesDataService>();
        builder.Services.AddScoped<ActingClientResolver>();
        builder.Services.AddSingleton(TimeProvider.System);
        builder.Services.AddSingleton<PendingFunctionCalls>();
        // TASK-ENG-013 replaces it with the port of orchestration
        builder.Services.AddSingleton<IFunctionCallResultPort, LoggingFunctionCallResultPort>();
        builder.Services.AddSingleton<SeedEnvironment>();

        // seeders run in this order: accounts and clients first, then what the other modules build on them
        builder.AddIdentityApi();
        builder.AddAuthorizationApi();
        builder.AddGroupsApi();
        builder.AddPluginsApi();
        builder.AddOrchestrationApi();
        builder.Services.AddHostedService<StartupSeedService>();

        foreach (var module in modulesRoots)
            module.Initialize(builder.Services, eventBus);

        configureServices?.Invoke(builder.Services);

        var app = builder.Build();

        app.UseAuthentication();
        app.UseAuthorization();
        app.UseAntiforgery();
        app.UseRateLimiter();
        // await app.StartAsync();
        // await Task.Delay(TimeSpan.FromSeconds(5));
        // foreach (var modulesRoot in modulesRoots)
        // {
        //    var cast = (ModuleCompositionRootBase)modulesRoot;
        //    cast.LogBenchmark();
        //
        // }
        // await app.StopAsync();

        app.MapIdentityApi();
        app.MapAuthorizationApi();
        app.MapGroupsApi();
        app.MapOrchestrationApi();
        app.MapPluginsApi();
        app.MapAccountAntiforgeryEndpoints();
        app.MapFunctionCallEndpoints();

        // only a client token connects; the server closes the connection when the token expires or the client is
        // revoked, the client refreshes the token over http and connects again
        app.MapHub<AppHub>(Connections.HubName, options => options.CloseOnAuthenticationExpiration = true)
            .RequireAuthorization(AuthPolicies.Client);

        mapExtra?.Invoke(app);
        return app;
    }
}
