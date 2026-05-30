using Dara.BuildingBlocks.Infrastructure.Abstractions;
using Dara.Modules.Connections.Application.Connections.CreateConnection;
using Dara.Modules.Connections.Application.Connections.DeleteConnection;
using Dara.Shared.Contracts;
using Microsoft.AspNetCore.SignalR;

namespace Dara.Apps.Server.API.AppHubs;

public partial class AppHub : Hub<IAppHubClient>, IAppHub
{
    private readonly IModuleCommandRunner _commandRunner;

    public AppHub(IModuleCommandRunner commandRunner)
    {
        _commandRunner = commandRunner;
    }

    public async override Task OnConnectedAsync()
    {
        string id = Context.ConnectionId;
        string ip = Context.GetHttpContext()!.Connection.RemoteIpAddress!.ToString();

        var command = new CreateConnectionCommand(id, ip);
        var result = await _commandRunner.ExecuteAsync(command);
    }

    public async override Task OnDisconnectedAsync(Exception? exception)
    {
        var command = new DeleteConnectionCommand(Context.ConnectionId);
        var result = await _commandRunner.ExecuteAsync(command);
    }
}
