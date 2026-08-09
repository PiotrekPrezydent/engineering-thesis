using System.Diagnostics;
using Dara.Server.Modules.Identity.Application;
using Dara.Server.Modules.Identity.Application.CreateClient;
using Dara.Server.Modules.Identity.Application.GetClient;
using Dara.Server.Modules.Profiles.Application;

namespace Dara.Server.Apps.API.Tests;

public class TestModules
{
    private readonly IIdentityModule _identityModule;
    private readonly IProfilesModule _profilesModule;

    public TestModules(IIdentityModule  identityModule)
    {
        //_identityModule = provider.GetRequiredService<IIdentityModule>();
        _identityModule = identityModule;
    }

    public async Task Start()
    {
        Console.WriteLine("START TESTING");
        
       // await TestIdentityModule("123");
       await TestProfileCreation();
        
        Console.WriteLine("STOP TESTING");
        
    }

    async Task TestIdentityModule(string userIndentifier)
    {
        var q = new GetClientQuery(userIndentifier);
        var dto = await _identityModule.ExecuteQueryAsync<GetClientQuery, ClientDto>(q);
        
        //var command = new CreateClientCommand(userIndentifier);
        
        //var id = await _identityModule.ExecuteCommandAsync<CreateClientCommand,Guid>(command);
        Console.WriteLine("RESOLVED ID : " + dto.ClientId);
    }


    async Task TestProfileCreation()
    {
        Stopwatch sw = Stopwatch.StartNew();
        Console.WriteLine("START: " + sw.ToString());
        
        var command = new CreateClientCommand("TEST");
        var g = await _identityModule.ExecuteCommandAsync<CreateClientCommand, Guid>(command);
        Console.WriteLine("POST COMMAND GUID: " + g);
        
        await Task.Delay(TimeSpan.FromSeconds(20));
    }
    
}