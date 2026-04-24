using Dara.BuildingBlocks.Application;
using Dara.BuildingBlocks.Infrastructure.Commands;
using Dara.Modules.Communication.Application.Connections.CreateConnection;
using Dara.Modules.Communication.Application.Connections.DeleteConnection;
using Dara.Shared.Common.Logging;
using Dara.Shared.Contracts;
using Microsoft.AspNetCore.SignalR;

namespace Dara.Apps.Server.API.AppHubs;

public partial class AppHub : Hub<IAppHubClient>, IAppHub
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
        Console.WriteLine("Connected: " + Context.ConnectionId);
        
        string id = Context.ConnectionId;
        string ip = Context.GetHttpContext()!.Connection.RemoteIpAddress!.ToString();

        var command = new CreateConnectionCommand(id, ip);
        var result = await _applicationCommandDispatcher.DispatchAsync<CreateConnectionCommand, CreateConnectionCommandResult>(command);

        if (result.Status != CommandResultStatus.Success)
        {
            var ex = result.ResultedException;
            throw ex;
        }
        
        await base.OnConnectedAsync();
    }

    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        Console.WriteLine("Disconnected: " + Context.ConnectionId);
        
        string id = Context.ConnectionId;

        var command = new DeleteConnectionCommand(id);
        var result = await _applicationCommandDispatcher.DispatchAsync<DeleteConnectionCommand, DeleteConnectionCommandResult>(command);
        
        if (result.Status != CommandResultStatus.Success)
        {
            var ex = result.ResultedException;
            throw ex;
        }
        
        await base.OnDisconnectedAsync(exception);
    }
    
}