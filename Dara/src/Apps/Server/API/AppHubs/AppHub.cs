using Dara.BuildingBlocks.Application;
using Dara.BuildingBlocks.Application.Abstraction;
using Dara.BuildingBlocks.Domain.Exceptions;
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
        string id = Context.ConnectionId;
        string ip = Context.GetHttpContext()!.Connection.RemoteIpAddress!.ToString();
        
        var command = new CreateClientCommand(id,ip);
        var result = await _applicationCommandDispatcher.DispatchAsync<CreateClientCommand, CreateClientCommandResult>(command);

        if (result.Status == CommandResultStatus.Success)
        {
            Console.WriteLine("Success");
        }
        
        await base.OnConnectedAsync();
    }

    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        // string id = Context.ConnectionId;
        //
        // try
        // {
        //     var command = new DeleteClientCommand();
        //     var result = await _applicationCommandDispatcher.DispatchAsync<DeleteClientCommand, DeleteClientCommandResult>(command);
        // }
        // catch (BaseDomainException ex)
        // {
        //     ex.PrintBuildedMessage();
        // }
        
        await base.OnDisconnectedAsync(exception);
    }
    
}