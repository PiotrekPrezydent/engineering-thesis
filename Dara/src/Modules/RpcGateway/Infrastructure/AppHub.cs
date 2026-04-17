using Dara.BuildingBlocks.Domain.Commands;
using Dara.Modules.RpcGateway.Contracts;
using Dara.Shared.Common.Logging;
using Dara.Shared.Contracts;
using Microsoft.AspNetCore.SignalR;

namespace Dara.Modules.RpcGateway.Infrastructure;

public class AppHub : Hub<IAppHubClient>, IAppHub
{
    private readonly IApplicationCommandDispatcher _applicationCommandDispatcher;
    private readonly ConsoleLogger _consoleLogger;
    public AppHub(IApplicationCommandDispatcher applicationCommandDispatcher)
    {
        _consoleLogger = new ConsoleLogger(this);
        _consoleLogger.Start("create");
        
        _applicationCommandDispatcher = applicationCommandDispatcher;
        
        _consoleLogger.End();
    }
    
    public override async Task OnConnectedAsync()
    {
        _consoleLogger.Start("on-connect");
        
        string id = Context.ConnectionId;
        string ip = Context.GetHttpContext()!.Connection.RemoteIpAddress!.ToString();
        
        var command = new ClientConnectedCommand(id,ip);
        var result = await _applicationCommandDispatcher.DispatchAsync<ClientConnectedCommand,ClientConnectedCommandResult>(command);
        
        await base.OnConnectedAsync();
        
        _consoleLogger.End();
    }

    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        _consoleLogger.Start("on-disconnect");
        
        string id = Context.ConnectionId;
        
        var command = new ClientDisconnectedCommand(id);
        var result = await _applicationCommandDispatcher.DispatchAsync<ClientDisconnectedCommand,ClientDisconnectedCommandResult>(command);
        
        await base.OnDisconnectedAsync(exception);
        
        _consoleLogger.End();
    }
}