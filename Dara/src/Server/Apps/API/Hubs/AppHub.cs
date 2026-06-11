using Microsoft.AspNetCore.SignalR;

namespace Dara.Server.Apps.API.Hubs;

public partial class AppHub : Hub
{

    public async override Task OnConnectedAsync()
    {
        string id = Context.ConnectionId;
        string ip = Context.GetHttpContext()!.Connection.RemoteIpAddress!.ToString();
        Console.WriteLine($"{Context.ConnectionId} connected to {ip} --- { Context.UserIdentifier}");
        // var command = new CreateConnectionCommand(id, ip);
        // var result = await _commandRunner.ExecuteAsync(command);
    }

    public async override Task OnDisconnectedAsync(Exception? exception)
    {
        // var command = new DeleteConnectionCommand(Context.ConnectionId);
        // var result = await _commandRunner.ExecuteAsync(command);
    }
}
