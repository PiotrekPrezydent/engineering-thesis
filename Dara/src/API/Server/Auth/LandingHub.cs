using Microsoft.AspNetCore.SignalR;

namespace Dara.API.Server.Auth;

public class LandingHub : Hub
{
    public override Task OnConnectedAsync()
    {
        Console.WriteLine($"connection ID: {Context.ConnectionId}");
        return base.OnConnectedAsync();
    }
    
    public override Task OnDisconnectedAsync(Exception? exception)
    {
        Console.WriteLine($"disconnected ID: {Context.ConnectionId}");
        return base.OnDisconnectedAsync(exception);
    }
}