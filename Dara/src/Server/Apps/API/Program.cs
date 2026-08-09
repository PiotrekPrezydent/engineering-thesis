using System.Security.Claims;
using Dara.Server.Apps.API.Authentication;
using Dara.Server.Apps.API.Events;
using Dara.Server.Apps.API.Hubs;
using Dara.Server.Apps.API.Tests;
using Dara.Server.BuildingBlocks.Infrastructure.Configuration;
using Dara.Server.BuildingBlocks.Infrastructure.Messaging.EventBus;
using Dara.Server.Modules.Groups.Application;
using Dara.Server.Modules.Groups.Infrastructure;
using Dara.Server.Modules.Identity.Application;
using Dara.Server.Modules.Identity.Infrastructure;
using Dara.Server.Modules.Profiles.Infrastructure;
using Dara.Shared.Contracts;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.SignalR;
using Microsoft.IdentityModel.Tokens.Experimental;

namespace Dara.Server.Apps.API;

public class Program
{
    public static async Task Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);
        
        builder.Logging
            .ClearProviders()
            .AddConsole(options =>
            {
                //options.FormatterName = nameof(DaraLogFormatter);
            });
        
        builder.Services.AddMemoryCache();
        
        builder.Services
            .AddSignalR(options =>
            {
                options.AddFilter<AppHubFilter>();
            });
        
        builder.Services
            .AddAuthentication(options =>
            {
                options.AddScheme<ClientIdentifierAuthHandler>(nameof(ClientIdentifierAuthHandler),null);
            });
        
        var modulesRoots = new IModuleCompositionRoot[]
        {
            new GroupCompositionRoot(),
            new IdentityCompositionRoot(),
            new ProfilesCompositionRoot()
        };
        
        foreach (var module in modulesRoots)
            module.Initialize(builder.Services);

        builder.Services.AddScoped<TestModules>();
        
        var app = builder.Build();
        
        var con = app.Services.GetRequiredService<IHubContext<AppHub,IAppHubClient>>();
        AppHub.InstanceContext = con;
        
        
        InMemoryEventBus.Instance.Subscribe(new ProfileNameChangedEventHandler(app.Services));
        InMemoryEventBus.Instance.Subscribe(new NewMemberJoinedGroupEventHandler(app.Services));
        InMemoryEventBus.Instance.Subscribe(new MemberLeftGroupEventHandler(app.Services));
        InMemoryEventBus.Instance.Subscribe(new GroupCreatedEventHandler(app.Services));
        
        app.UseAuthentication();
        app.UseAuthorization();
        
        // using (var scope = app.Services.CreateScope())
        // {
        //     app.Start();
        //     var test = scope.ServiceProvider.GetRequiredService<TestModules>();
        //     await test.Start();
        //
        //     //await Task.Delay(200000);
        // }


        
        app.MapHub<AppHub>(Connections.HubName);
        app.Run();
    }
}
