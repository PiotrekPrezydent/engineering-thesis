using Dara.Modules.RpcGateway.Domain;
using Dara.Shared.Contracts;
using Microsoft.AspNetCore.SignalR;

namespace Dara.Modules.RpcGateway.Infrastructure;

public class AppHub : Hub<IAppHubClient>, IAppHub
{
    private List<ClientConnection> _clients;

    public AppHub()
    {
        _clients = new();
    }
    
    public override Task OnConnectedAsync()
    {
        Console.WriteLine("CONNECTED:" + Context.ConnectionId );
        //add client
        return base.OnConnectedAsync();
    }

    public override Task OnDisconnectedAsync(Exception? exception)
    {
        Console.WriteLine("DISCONNECTED:" + Context.ConnectionId );
        //remove client
        return base.OnDisconnectedAsync(exception);
    }
}