using Dara.Server.Modules.Groups.Application;
using Dara.Server.Modules.Groups.Application.Groups.CreateNewGroup;
using Dara.Server.Modules.Groups.Application.Groups.JoinToGroup;
using Dara.Server.Modules.Identity.Application;
using Dara.Server.Modules.Identity.Application.CreateUser;
using Dara.Server.Modules.Plugins.Application;
using Dara.Server.Modules.Plugins.Application.AddPlugin;
using Dara.Server.Modules.Plugins.Application.Data;
using Dara.Server.Modules.Profiles.Application;
using Dara.Server.Modules.Profiles.Application.ChangeProfileName;

namespace Dara.Server.Apps.API.Services;

public class StartupService : IHostedLifecycleService
{
    private readonly ILogger<StartupService> _logger;
    private readonly IIdentityModule _identityModule;
    private readonly IProfilesModule _profilesModule;
    private readonly IGroupsModule _groupsModule;
    private readonly IPluginsModule _pluginsModule;
    
    private readonly LogModulesDataService _logModulesDataService;


    public StartupService(ILogger<StartupService> logger, IServiceProvider serviceProvider)
    {
        _logger = logger;
        using var scope = serviceProvider.CreateScope();
        
        _identityModule = scope.ServiceProvider.GetRequiredService<IIdentityModule>();
        _profilesModule = scope.ServiceProvider.GetRequiredService<IProfilesModule>();
        _groupsModule = scope.ServiceProvider.GetRequiredService<IGroupsModule>();
        _pluginsModule = scope.ServiceProvider.GetRequiredService<IPluginsModule>();
        
        _logModulesDataService = scope.ServiceProvider.GetRequiredService<LogModulesDataService>();
    }
    
    public Task StartingAsync(CancellationToken cancellationToken) => Task.CompletedTask;
    public Task StartAsync(CancellationToken cancellationToken) => Task.CompletedTask;

    public async Task StartedAsync(CancellationToken cancellationToken)
    {
        var user1 = await _identityModule.ExecuteCommandAsync<CreateUserCommand, Guid>(new CreateUserCommand("user-1"));
        var user2 = await _identityModule.ExecuteCommandAsync<CreateUserCommand, Guid>(new CreateUserCommand("user-2"));
        var user3 = await _identityModule.ExecuteCommandAsync<CreateUserCommand, Guid>(new CreateUserCommand("user-3"));
        var user4 = await _identityModule.ExecuteCommandAsync<CreateUserCommand, Guid>(new CreateUserCommand("user-4"));
        var user5 = await _identityModule.ExecuteCommandAsync<CreateUserCommand, Guid>(new CreateUserCommand("user-5"));
        
        //wait for default user representations are created
        await Task.Delay(TimeSpan.FromSeconds(1));

        await SetupProfileAsync(user1, "Bob-Phone");
        await SetupProfileAsync(user2, "Bob-Laptop");
        await SetupProfileAsync(user3, "Bob-Smartwatch");
        await SetupProfileAsync(user4, "Room-101-Computer-01");
        await SetupProfileAsync(user5, "Room-101-Computer-02");
        
        var group1 = await SetupGroupAsync(user1, "group-1","group-1-jc", user2,user3,user4,user5);
        var group2 = await SetupGroupAsync(user2, "group-2","group-2-jc", user3,user4,user5);
        var group3 = await SetupGroupAsync(user3, "group-3","group-3-jc", user4,user5);

        await SetupPluginsAsync(user1,
            SamplePlugins.CameraPlugin(),
            SamplePlugins.FileManagementPlugin(),
            SamplePlugins.DisplayPlugin(),
            SamplePlugins.NotificationsPlugin(),
            SamplePlugins.LocationPlugin());

        await SetupPluginsAsync(user2,
            SamplePlugins.CameraPlugin(),
            SamplePlugins.FileManagementPlugin(),
            SamplePlugins.DisplayPlugin(),
            SamplePlugins.NotificationsPlugin(),
            SamplePlugins.LocationPlugin(),
            SamplePlugins.SystemControlPlugin());
        
        await SetupPluginsAsync(user3,
            SamplePlugins.DisplayPlugin(),
            SamplePlugins.NotificationsPlugin(),
            SamplePlugins.LocationPlugin());

        await SetupPluginsAsync(user4,
            SamplePlugins.DisplayPlugin(),
            SamplePlugins.NotificationsPlugin(),
            SamplePlugins.SystemControlPlugin());
        
        await SetupPluginsAsync(user5,
            SamplePlugins.DisplayPlugin(),
            SamplePlugins.NotificationsPlugin(),
            SamplePlugins.SystemControlPlugin());

        
        //wait for pending inbox/outbox messagess to process
        await Task.Delay(TimeSpan.FromSeconds(2));
        
        await _logModulesDataService.LogDataAsync();

    }
    
    public Task StoppingAsync(CancellationToken cancellationToken) => Task.CompletedTask;
    public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;
    public Task StoppedAsync(CancellationToken cancellationToken) => Task.CompletedTask;

    async Task SetupProfileAsync(Guid profileId, string profileName)
    {
        await _profilesModule.ExecuteCommandAsync(new ChangeProfileNameCommand(profileId, profileName));
    }
    
    async Task<Guid> SetupGroupAsync(Guid creatorId, string name,string joinCode, params Guid[] membersIds)
    {
        var group = await _groupsModule.ExecuteCommandAsync<CreateNewGroupCommand, Guid>(new CreateNewGroupCommand(creatorId,name,joinCode));
        foreach (var memberId in membersIds)
        {
            await _groupsModule.ExecuteCommandAsync(new JoinToGroupCommand(group, memberId, joinCode));
        }

        return group;
    }

    async Task SetupPluginsAsync(Guid ownerId, params PluginDescriptor[] plugins)
    {
        foreach (var plugin in plugins)
        {
            await _pluginsModule.ExecuteCommandAsync(new AddPluginCommand(ownerId, plugin));
        }
    }

    

}