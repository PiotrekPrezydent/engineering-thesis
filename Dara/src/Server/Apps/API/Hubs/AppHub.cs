using System.Collections.Concurrent;
using Dara.Server.Modules.Clients.Application;
using Dara.Server.Modules.Clients.Application.Clients.CreateClient;
using Dara.Server.Modules.Clients.Application.Clients.GetClient;
using Dara.Server.Modules.Clients.Application.Clients.MarkClientAsOffline;
using Dara.Server.Modules.Clients.Application.Clients.MarkClientAsOnline;
using Dara.Shared.Contracts.Hubs;
using Microsoft.AspNetCore.SignalR;

namespace Dara.Server.Apps.API.Hubs;

public partial class AppHub : Hub, IAppHub
{
    //store connection to hub and session id
    static ConcurrentDictionary<Guid, string> _connections = new();
    private readonly IClientsModule _clientsModule;
    Guid CallerId => Guid.Parse(Context.UserIdentifier!);

    public AppHub(IClientsModule clientsModule)
    {
        _clientsModule = clientsModule;
    }

    public override async Task OnConnectedAsync()
    {
        if (_connections.TryGetValue(CallerId, out var connectionId))
        {
            _connections[CallerId] = connectionId;
            var command = new MarkClientAsOnlineCommand(CallerId);
            
            await _clientsModule.ExecuteCommandAsync(command);
        }
        else
        {
            _connections.TryAdd(CallerId, Context.ConnectionId);
            var command = new CreateClientCommand(CallerId, Context.ConnectionId);
            
            await _clientsModule.ExecuteCommandAsync(command);
        }
        await base.OnConnectedAsync();
    }

    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        var query = new GetClientQuery(CallerId);
        var result = await _clientsModule.ExecuteQueryAsync<GetClientQuery, ClientDto>(query);
        if (result.IsOnline)
        {
            var command = new MarkClientAsOfflineCommand(CallerId);
        
            await _clientsModule.ExecuteCommandAsync(command);
        }

        
        await base.OnDisconnectedAsync(exception);
    }
}
