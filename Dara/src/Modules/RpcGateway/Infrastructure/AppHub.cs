using Dara.BuildingBlocks.Domain.Commands;
using Dara.Modules.RpcGateway.Contracts;
using Dara.Shared.Contracts;
using Microsoft.AspNetCore.SignalR;

namespace Dara.Modules.RpcGateway.Infrastructure;

public class AppHub : Hub<IAppHubClient>, IAppHub
{
    private readonly IApplicationCommandDispatcher _applicationCommandDispatcher;
    
    public AppHub(IApplicationCommandDispatcher applicationCommandDispatcher)
    {
        _applicationCommandDispatcher = applicationCommandDispatcher;
    }
    
    public override async Task OnConnectedAsync()
    {
        Console.WriteLine("########## CONNECTED: " + Context.ConnectionId );
        
        string id = Context.ConnectionId;
        string ip = Context.GetHttpContext()!.Connection.RemoteIpAddress!.ToString();
        
        var command = new ClientConnectedCommand(id,ip);
        var result = await _applicationCommandDispatcher.DispatchAsync<ClientConnectedCommand,ClientConnectedCommandResult>(command);
        
        Console.WriteLine(result);
        
        await base.OnConnectedAsync();
    }

    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        Console.WriteLine("########## DISCONNECTED: " + Context.ConnectionId );
        
        string id = Context.ConnectionId;
        
        var command = new ClientDisconnectedCommand(id);
        var result = await _applicationCommandDispatcher.DispatchAsync<ClientDisconnectedCommand,ClientDisconnectedCommandResult>(command);
        
        Console.WriteLine(result);
        
        await base.OnDisconnectedAsync(exception);
    }
}