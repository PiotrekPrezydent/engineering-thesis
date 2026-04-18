using Dara.BuildingBlocks.Domain.Commands;
using Dara.Modules.RpcGateway.Contracts;
using Dara.Shared.Common.Logging;
using Dara.Shared.Contracts;
using Dara.Shared.Contracts.Connection;
using Microsoft.AspNetCore.SignalR;

namespace Dara.Modules.RpcGateway.Infrastructure;

public class AppHub : Hub<IAppHubClient>, IAppHub
{
    private readonly IApplicationCommandDispatcher _applicationCommandDispatcher;
    private readonly ConsoleLogger _consoleLogger;
    
    public AppHub(IApplicationCommandDispatcher applicationCommandDispatcher)
    {
        _consoleLogger = new ConsoleLogger(this);
        _applicationCommandDispatcher = applicationCommandDispatcher;
    }
    
    public override async Task OnConnectedAsync()
    {
        string id = Context.ConnectionId;
        string ip = Context.GetHttpContext()!.Connection.RemoteIpAddress!.ToString();

        var command = new SetConnectionEstablishedCommand(id,ip);
        var result = await _applicationCommandDispatcher.DispatchAsync<SetConnectionEstablishedCommand, SetConnectionEstablishedCommandResult>(command);
        
        await base.OnConnectedAsync();
    }

    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        string id = Context.ConnectionId;

        var command = new SetConnectionLostCommand(id);
        var result = await _applicationCommandDispatcher.DispatchAsync<SetConnectionLostCommand, SetConnectionLostCommandResult>(command);
        
        await base.OnDisconnectedAsync(exception);
    }

    public async Task BroadcastMessageAsync(MessageDto message)
    {
        await Clients.All.ReceiveMessageAsync(message);
    }

    public async Task BroadcastMessageAsync(MessageDto message, params string[] connectionIds)
    {
        await Clients.Clients(connectionIds).ReceiveMessageAsync(message);
    }

    public async Task BrodcastMessageAsync(MessageDto message, string connectionIp)
    {
        var command = new GetIpConnectionsCommand(connectionIp);
        var result = await _applicationCommandDispatcher.DispatchAsync<GetIpConnectionsCommand, GetIpConnectionsCommandResult>(command);
        
        await Clients.Clients(result.ConnectionIds).ReceiveMessageAsync(message);
    }
}