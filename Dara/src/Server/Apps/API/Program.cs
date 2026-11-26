using System.Collections.Immutable;
using System.Reflection;
using System.Threading.Channels;
using System.Threading.RateLimiting;
using Dara.Server.Apps.API.Authentication;
using Dara.Server.Apps.API.Endpoints;
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
using Dara.Shared.Contracts.Auth;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.SignalR;
using Microsoft.IdentityModel.Tokens;

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

    // modules build their containers before the host, so tests pass their own roots here (e.g. a scripted model provider);
    // mapExtra maps endpoints of a test host (e.g. probes of the authorization policies)
    public static WebApplication CreateApplication(string[] args, IReadOnlyList<IModuleCompositionRoot> modulesRoots, Action<WebApplication>? mapExtra = null)
    {
        var builder = WebApplication.CreateBuilder(args);

        builder.Services.AddSignalR(options =>
        {
            options.AddFilter<AppHubFilter>();
        });
        builder.Services.AddSingleton<IUserIdProvider, SubjectUserIdProvider>();

        AddAuthentication(builder);
        AddDeviceLoginRateLimit(builder);
        // minimal api checks the token only on endpoints that bind a form (the panel, TASK-ENG-036);
        // json endpoints of the panel are covered by the SameSite=Strict cookie
        builder.Services.AddAntiforgery();
        builder.Services.AddProblemDetails();

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
        
        app.MapAuthEndpoints();

        // only a device token connects; the server closes the connection when the token expires,
        // the client refreshes it over http and connects again
        app.MapHub<AppHub>(Connections.HubName, options => options.CloseOnAuthenticationExpiration = true)
            .RequireAuthorization(AuthPolicies.Device);

        mapExtra?.Invoke(app);
        return app;
    }

    // two schemes: the device token (jwt, the default) for devices and the hub, the cookie for the account panel
    static void AddAuthentication(WebApplicationBuilder builder)
    {
        var jwt = JwtOptions.FromConfiguration(builder.Configuration);
        builder.Services.AddSingleton(jwt);
        builder.Services.AddSingleton(TimeProvider.System);
        builder.Services.AddSingleton<AccessTokenIssuer>();

        builder.Services
            .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                // claims keep their jwt names (sub, scope, device, sid)
                options.MapInboundClaims = false;
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidIssuer = jwt.Issuer,
                    ValidateAudience = true,
                    ValidAudience = jwt.Audience,
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = jwt.CreateSigningKey(),
                    ValidAlgorithms = [SecurityAlgorithms.HmacSha256],
                    ValidateLifetime = true,
                    // one server issues and checks, without it an expired token would work 5 more minutes
                    ClockSkew = TimeSpan.Zero,
                    NameClaimType = AuthClaims.Subject
                };
                options.Events = new JwtBearerEvents
                {
                    // a browser cannot set headers on a websocket, so the hub also takes the token from the query
                    OnMessageReceived = context =>
                    {
                        var token = context.Request.Query["access_token"];
                        if (!string.IsNullOrEmpty(token) && context.HttpContext.Request.Path.StartsWithSegments("/" + Connections.HubName))
                            context.Token = token;

                        return Task.CompletedTask;
                    }
                };
            })
            .AddCookie(AccountCookie.Scheme, AccountCookie.Configure);

        builder.Services.AddAuthorization(AuthPolicies.Configure);
    }

    // attempts of device-login per client address in a fixed window of one minute (Auth:DeviceLoginPermitsPerMinute)
    static void AddDeviceLoginRateLimit(WebApplicationBuilder builder)
    {
        var permits = builder.Configuration.GetValue("Auth:DeviceLoginPermitsPerMinute", 10);
        if (permits <= 0)
            throw new InvalidOperationException("Auth__DeviceLoginPermitsPerMinute must be positive");

        builder.Services.AddRateLimiter(options =>
        {
            options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;
            options.AddPolicy(AuthEndpoints.DeviceLoginRateLimit, context => RateLimitPartition.GetFixedWindowLimiter(
                context.Connection.RemoteIpAddress?.ToString() ?? "unknown",
                _ => new FixedWindowRateLimiterOptions
                {
                    PermitLimit = permits,
                    Window = TimeSpan.FromMinutes(1),
                    QueueLimit = 0
                }));
        });
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
