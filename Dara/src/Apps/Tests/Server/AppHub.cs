using Microsoft.AspNetCore.SignalR;

namespace Dara.Apps.Tests.Server;

public class AppHub : Hub<IAppClient>
{
    public override Task OnConnectedAsync()
    {
        Console.WriteLine($"{Context.ConnectionId} connected");
        
        
        return base.OnConnectedAsync();
    }

    public override Task OnDisconnectedAsync(Exception? exception)
    {
        Console.WriteLine($"{Context.ConnectionId} disconnected");
        
        
        return base.OnDisconnectedAsync(exception);
    }

    //login to account using domain, then add this 
    public Task LoginToAccount(string username, string password)
    {
        return Task.CompletedTask;
    }
    
    //save in domain who is in which group
    public Task JoinNodesGroup(string groupName)
    {
        
        return Task.CompletedTask;
    }

    public Task LeaveNodesGroup(string groupName)
    {
        return Task.CompletedTask;
    }
    
    //rest api
    // public Task CreateNodesGroup(string groupName)
    // {
    //     return Task.CompletedTask;
    // }

    public Task SendGroupPromptMessage(string groupName, string message)
    {
        return Task.CompletedTask;
    }
    
    
}