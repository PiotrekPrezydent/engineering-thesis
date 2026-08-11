using System.Threading.Channels;
using Dara.Server.Apps.API.Authentication;
using Dara.Server.Apps.API.Hubs;
using Dara.Server.Apps.API.Processing;
using Dara.Server.Apps.API.Tests;
using Dara.Server.BuildingBlocks.Infrastructure.Configuration;
using Dara.Server.BuildingBlocks.Infrastructure.Messaging.EventBus;
using Dara.Server.BuildingBlocks.Integration;
using Dara.Server.Modules.Groups.Infrastructure;
using Dara.Server.Modules.Identity.Infrastructure;
using Dara.Server.Modules.Profiles.Infrastructure;
using Dara.Shared.Contracts;
using Dara.Shared.Logging;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging.Console;

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
                options.FormatterName = nameof(SharedLogFormatter);
            });
        
        builder.Logging.AddConsoleFormatter<SharedLogFormatter, ConsoleFormatterOptions>();
        
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
            new GroupsCompositionRoot(),
            new IdentityCompositionRoot(),
            new ProfilesCompositionRoot()
        };
        
        
        foreach (var module in modulesRoots)
            module.Initialize(builder.Services, InMemoryEventBus.Instance);

        builder.Services.AddScoped<TestModules>();

        //builder.Services.AddScoped<IHubNotificationHandler<ProfileNameChangedIntegrationEvent>,ProfileNameChangedNotificationHandler>();
        //builder.Services.AddScoped<IHubNotificationHandler<GroupCreatedIntegrationEvent>,GroupCreatedNotificationHandler>();
        //builder.Services.AddScoped<IHubNotificationHandler<NewMemberJoinedGroupIntegrationEvent>,GroupMemberJoinedGroupNotificationHandler >();
        //builder.Services.AddScoped<IHubNotificationHandler<MemberLeftGroupIntegrationEvent>,GroupMemberLeftGroupNotificationHandler>();
        //builder.Services.AddScoped<IHubNotificationHandler<NewGroupMessageCreatedIntegrationEvent>,NewGroupMessageNotificationHandler>();
        
        builder.Services.AddSingleton(Channel.CreateUnbounded<IIntegrationEvent>());
        builder.Services.AddHostedService<HubNotificationsProcessor>();
        
        var app = builder.Build();
        
        var channel = app.Services.GetRequiredService<Channel<IIntegrationEvent>>();
        
        // InMemoryEventBus.Instance.Subscribe(new ChannelWriterIntegrationEventHandler<ProfileNameChangedIntegrationEvent>(channel));
        // InMemoryEventBus.Instance.Subscribe(new ChannelWriterIntegrationEventHandler<NewMemberJoinedGroupIntegrationEvent>(channel));
        // InMemoryEventBus.Instance.Subscribe(new ChannelWriterIntegrationEventHandler<MemberLeftGroupIntegrationEvent>(channel));
        // InMemoryEventBus.Instance.Subscribe(new ChannelWriterIntegrationEventHandler<GroupCreatedIntegrationEvent>(channel));
        // InMemoryEventBus.Instance.Subscribe(new ChannelWriterIntegrationEventHandler<NewGroupMessageCreatedIntegrationEvent>(channel));
        
        app.UseAuthentication();
        app.UseAuthorization();
        
        using (var scope = app.Services.CreateScope())
        {
            app.Start();
            var test = scope.ServiceProvider.GetRequiredService<TestModules>();
            await test.Start();
        
            //await Task.Delay(200000);
        }
        
        app.MapHub<AppHub>(Connections.HubName);
        app.Run();
    }
}
