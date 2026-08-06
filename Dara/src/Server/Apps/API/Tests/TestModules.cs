using Dara.Server.Modules.Identity.Application;
using Dara.Server.Modules.Identity.Application.ResolveUserId;

namespace Dara.Server.Apps.API.Tests;

public class TestModules
{
    private readonly IIdentityModule _identityModule;

    public TestModules(IIdentityModule  identityModule)
    {
        //_identityModule = provider.GetRequiredService<IIdentityModule>();
        _identityModule = identityModule;
    }

    public async Task Start()
    {
        Console.WriteLine("START TESTING");
        
        await TestIdentityModule("123");
        await TestIdentityModule("123");
        
        Console.WriteLine("STOP TESTING");
    }

    async Task TestIdentityModule(string userIndentifier)
    {
        var command = new ResolveUserIdCommand(userIndentifier);
        
        var id = await _identityModule.ExecuteCommandAsync<ResolveUserIdCommand,Guid>(command);
        Console.WriteLine("RESOLVED ID : " + id.ToString());
    }
    
}