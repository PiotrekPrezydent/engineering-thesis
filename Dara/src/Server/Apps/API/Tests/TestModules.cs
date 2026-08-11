using Dara.Server.Modules.Groups.Application;
using Dara.Server.Modules.Groups.Application.CreateGroup;
using Dara.Server.Modules.Groups.Application.JoinToGroup;
using Dara.Server.Modules.Groups.Application.LeaveGroup;
using Dara.Server.Modules.Groups.Application.SendGroupMessage;
using Dara.Server.Modules.Identity.Application;
using Dara.Server.Modules.Identity.Application.CreateUser;
using Dara.Server.Modules.Identity.Application.GetUser;
using Dara.Server.Modules.Profiles.Application;
using Dara.Server.Modules.Profiles.Application.ChangeProfileName;

namespace Dara.Server.Apps.API.Tests;

public class TestModules
{
    private readonly IIdentityModule _identityModule;
    
    private readonly IProfilesModule _profilesModule;
    private readonly IGroupsModule _groupModule;

    public TestModules(IIdentityModule  identityModule, IProfilesModule profilesModule, IGroupsModule groupModule)
    {
        //_identityModule = provider.GetRequiredService<IIdentityModule>();
        _identityModule = identityModule;
        _profilesModule = profilesModule;
        _groupModule = groupModule;
    }

    public async Task Start()
    {
        var clients = new List<Guid>();
        for (int i = 0; i < 5; i++)
        {
            Console.WriteLine("START LOOP " + i);
            var command1 = new CreateUserCommand("TEST"+i);
            
            var  g = await _identityModule.ExecuteCommandAsync<CreateUserCommand, Guid>(command1);
            clients.Add(g);
            Console.WriteLine("END LOOP " + i);
        }
        
        Console.WriteLine("WAIT FOR PROFILES");
        await Task.Delay(TimeSpan.FromSeconds(2));
        Console.WriteLine("WAIT FOR PROFILES END");

        foreach (var client in clients)
        {
            var command = new ChangeProfileNameCommand(client, "str1234");
            await _profilesModule.ExecuteCommandAsync(command);
        }
        
        
        var group = await _groupModule.ExecuteCommandAsync<CreateGroupCommand,Guid>(new CreateGroupCommand(clients[0], "NAME"));
        string code = "GROUP-0";
        foreach (var client in clients.Skip(1))
        {
            var command = new JoinToGroupCommand(group, client, code);
            await _groupModule.ExecuteCommandAsync(command);
        }

        foreach (var client in clients)
        {
            var command = new SendGroupMessageCommand(group, client, "MSGGG");
            await _groupModule.ExecuteCommandAsync(command);
        }

        foreach (var client in clients)
        {
            var command = new LeaveGroupCommand(group, client);
            await _groupModule.ExecuteCommandAsync(command);
        }
        
        
        //
        // var grid = await _groupModule.ExecuteCommandAsync<CreateGroupCommand,Guid>(new CreateGroupCommand(clients[0], "NAME"));
        // for (int i = 1; i < 10; i++)
        // {
        //     await _groupModule.ExecuteCommandAsync(new JoinToGroupCommand(grid,clients[i],code));
        // }
        //
        //
        // await _groupModule.ExecuteCommandAsync(new SendMessageToGroupCommand(grid,clients[0],"SOME MESSAGE"));
        
        
        await Task.Delay(TimeSpan.FromSeconds(20));
       // await TestIdentityModule("123");
       //await TestProfileCreation();
        
        Console.WriteLine("STOP TESTING");
        
    }

    async Task TestIdentityModule(string userIndentifier)
    {
        var q = new GetUserQuery(userIndentifier);
        var dto = await _identityModule.ExecuteQueryAsync<GetUserQuery, Guid?>(q);
        
        //var command = new CreateClientCommand(userIndentifier);
        
        //var id = await _identityModule.ExecuteCommandAsync<CreateClientCommand,Guid>(command);
        Console.WriteLine("RESOLVED ID : " + dto);
    }


    // async Task TestProfileCreation()
    // {
    //     Stopwatch sw = Stopwatch.StartNew();
    //     Console.WriteLine("START: " + sw.ToString());
    //     
    //     var command = new CreateClientIdentityCommand("TEST");
    //     var g = await _identityModule.ExecuteCommandAsync<CreateClientIdentityCommand, Guid>(command);
    //     Console.WriteLine("POST COMMAND GUID: " + g);
    //     
    //     await Task.Delay(TimeSpan.FromSeconds(2));
    //
    //     await _profilesModule.ExecuteCommandAsync(new ChangeProfileNameCommand(g, "1234"));
    //     await Task.Delay(TimeSpan.FromSeconds(2));
    // }
    
}