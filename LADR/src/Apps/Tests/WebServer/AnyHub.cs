using Microsoft.AspNetCore.SignalR;

namespace LADR.Tests.WebServer;

public class AnyHub : Hub
{
    public override Task OnConnectedAsync()
    {
        Console.WriteLine($"conn ID: {Context.ConnectionId}");
        return base.OnConnectedAsync();
    }
    
    public override Task OnDisconnectedAsync(Exception? exception)
    {
        Console.WriteLine($"discon ID: {Context.ConnectionId}");
        return base.OnDisconnectedAsync(exception);
    }
    
    public async Task SendMessageToServer(string message)
    {
        Console.WriteLine($"receivedmsg: {message}");
        
        await Clients.All.SendAsync("ReceiveMessage", $"Serwer received msg: {message}");
    }
}