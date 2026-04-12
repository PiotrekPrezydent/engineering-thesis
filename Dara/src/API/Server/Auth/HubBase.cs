using Dara.Core.Domain.Commands;
using Microsoft.AspNetCore.SignalR;

namespace Dara.API.Server.Auth;

public class HubBase<T> : Hub<T> where T : class
{
    protected IApplicationCommandDispatcher _commandDispatcher;

    public HubBase(IApplicationCommandDispatcher commandDispatcher)
    {
        _commandDispatcher = commandDispatcher;
    }
    
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