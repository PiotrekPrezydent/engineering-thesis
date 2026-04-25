using Dara.BuildingBlocks.Application;
using Dara.BuildingBlocks.Application.Integration;
using Dara.Modules.Connections.Application.Clients.ChangeClientAuthToken;
using Dara.Modules.Connections.Application.Clients.ChangeClientName;
using Dara.Modules.Connections.Application.Clients.CreateClient;
using Dara.Modules.Connections.Application.Clients.DeleteClient;
using Dara.Modules.Connections.Application.Clients.GetClient;
using Dara.Modules.Connections.Application.Connections.CreateConnection;
using Dara.Modules.Connections.Application.Connections.DeleteConnection;
using Dara.Modules.Connections.Application.Domain;

namespace Dara.Modules.Connections.Application;

public class ConnectionsApplicationLayer : IApplicationLayer
{
    public IReadOnlyList<Type> GetModuleCommandHandlers => [
        typeof(CreateConnectionCommandHandler),
        typeof(DeleteConnectionCommandHandler),
        typeof(CreateClientCommandHandler),
        typeof(DeleteClientCommandHandler),
        typeof(GetClientCommandHandler),
        typeof(ChangeClientNameCommandHandler),
        typeof(ChangeClientAuthTokenCommandHandler)
    ];
    
    public IReadOnlyList<Type> GetDomainEventHandlers => [
        typeof(ConnectionCreatedEventHandler),
        typeof(ConnectionDeletedEventHandler),
        typeof(ClientCreatedEventHandler),
        typeof(ClientDeletedEventHandler),
        typeof(ClientNameChangedEventHandler),
        typeof(ClientAuthTokenChangedEventHandler)
    ];
    
    public IReadOnlyList<IIntegrationEventHandler> GetIntegrationEventHandlers => [
    
    ];
}