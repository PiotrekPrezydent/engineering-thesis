using Dara.BuildingBlocks.Application;
using Dara.BuildingBlocks.Application.Abstraction;
using Dara.BuildingBlocks.Infrastructure.Commands;
using Dara.Modules.Communication.Application.Clients;
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
        
        var command = new CreateClientCommand(id,ip);
        
        var result = await _applicationCommandDispatcher.DispatchAsync<CreateClientCommand, CreateClientCommandResult>(command);

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
        var command = new DeleteClientByConnectionIdCommand(id);
        var result = await _applicationCommandDispatcher.DispatchAsync<DeleteClientCommand, DeleteClientCommandResult>(command);
        
        if (result.Status != CommandResultStatus.Success)
        {
            var ex = result.ResultedException;
            throw ex;
        }
        
        await base.OnDisconnectedAsync(exception);
    }
    
}