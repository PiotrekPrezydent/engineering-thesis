using Dara.Server.Modules.Identity.Application;
using Hub = Microsoft.AspNetCore.SignalR.Hub;

namespace Dara.Server.Apps.API.Hubs;

public partial class AppHub : Hub
{
    private readonly IIdentityModule _identityModule;

    public AppHub(IIdentityModule identityModule)
    {
        _identityModule = identityModule;
    }

    public override async Task OnConnectedAsync()
    {
        Console.WriteLine($"{Context.ConnectionId} connected - GUID: {Context.GuidIdentifier()}");
        
        await base.OnConnectedAsync();
    }

    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        await base.OnDisconnectedAsync(exception);
    }

    public async Task Test()
    {
        await Task.CompletedTask;
    }
}
