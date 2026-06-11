using System.Collections.Concurrent;
using Dara.Server.Modules.Clients.Application;
using Dara.Server.Modules.Clients.Application.Clients.CreateClient;
using Dara.Server.Modules.Clients.Application.Clients.GetClient;
using Dara.Server.Modules.Clients.Application.Clients.MarkClientAsOffline;
using Dara.Server.Modules.Clients.Application.Clients.MarkClientAsOnline;
using Microsoft.AspNetCore.SignalR;

namespace Dara.Server.Apps.API.Hubs;

public partial class AppHub : Hub
{
    //store connection to hub and session id
    static ConcurrentDictionary<Guid, string> _connections = new();
    private readonly IClientsModule _clientsModule;

    public AppHub(IClientsModule clientsModule)
    {
        _clientsModule = clientsModule;
    }

    public async override Task OnConnectedAsync()
    {
        Console.WriteLine($"{Context.ConnectionId} --- { Context.UserIdentifier}");
        
        var clientId = Guid.Parse(Context.UserIdentifier!);
        if (_connections.TryGetValue(clientId, out var connectionId))
        {
            _connections[clientId] = connectionId;
            var command = new MarkClientAsOnlineCommand(clientId);
            
            await _clientsModule.ExecuteCommandAsync(command);
        }
        else
        {
            _connections.TryAdd(clientId, Context.ConnectionId);
            var command = new CreateClientCommand(clientId, Context.ConnectionId);
            
            await _clientsModule.ExecuteCommandAsync(command);
        }
        await base.OnConnectedAsync();
    }

    public async override Task OnDisconnectedAsync(Exception? exception)
    {
        var clientId = Guid.Parse(Context.UserIdentifier!);
        
        var query = new GetClientQuery(clientId);
        var result = await _clientsModule.ExecuteQueryAsync<GetClientQuery, ClientDto>(query);
        if (result.IsOnline)
        {
            var command = new MarkClientAsOfflineCommand(clientId);
        
            await _clientsModule.ExecuteCommandAsync(command);
        }

        
        await base.OnDisconnectedAsync(exception);
    }
}
