using Dara.Shared.Contracts;
using Microsoft.AspNetCore.SignalR;

namespace Dara.Apps.Tests.Server;

public class AppHub : Hub<IAppClient>
{
    public override Task OnConnectedAsync()
    {
        Console.WriteLine($"{Context.ConnectionId} connected");
        
        string connectionId = Context.ConnectionId;
        string? userId = Context.UserIdentifier;
        Console.WriteLine($"1: {connectionId} -- {userId}");
        
        var httpContext = Context.GetHttpContext();
        Console.WriteLine($"2: {httpContext}");
        
        if (httpContext != null)
        {
            // ip
            string? ipAddress = httpContext.Connection.RemoteIpAddress?.ToString();

            // B: User-Agent 
            string? userAgent = httpContext.Request.Headers["User-Agent"].ToString();

            // C: query string
            string? myCustomParam = httpContext.Request.Query["myParam"].ToString();
            
            Console.WriteLine($"3:{ipAddress} - {userAgent}  - {myCustomParam}");
        }
        Console.WriteLine("\n\n");
        
        
        return base.OnConnectedAsync();
    }

    public override Task OnDisconnectedAsync(Exception? exception)
    {
        Console.WriteLine($"{Context.ConnectionId} disconnected");
        
        
        return base.OnDisconnectedAsync(exception);
    }

    
    
}