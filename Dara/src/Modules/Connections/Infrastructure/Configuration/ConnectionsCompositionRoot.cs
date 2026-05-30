using Dara.BuildingBlocks.Application;
using Dara.BuildingBlocks.Infrastructure;
using Dara.BuildingBlocks.Infrastructure.Abstractions;
using Dara.BuildingBlocks.Infrastructure.Configuration;
using Dara.Modules.Connections.Application.Clients.ChangeClientAuthToken;
using Dara.Modules.Connections.Application.Clients.ChangeClientName;
using Dara.Modules.Connections.Application.Clients.CreateClient;
using Dara.Modules.Connections.Application.Clients.DeleteClient;
using Dara.Modules.Connections.Application.Clients.GetClient;
using Dara.Modules.Connections.Application.Connections.CreateConnection;
using Dara.Modules.Connections.Application.Connections.DeleteConnection;
using Dara.Modules.Connections.Application.Domain;
using Dara.Modules.Connections.Domain.Clients.Events;
using Dara.Modules.Connections.Domain.Connections.Events;
using Microsoft.Extensions.DependencyInjection;

namespace Dara.Modules.Connections.Infrastructure.Configuration;

public class ConnectionsCompositionRoot
{
    public static void Initialize(IServiceCollection services)
    {
        services.AddSingleton<IHandler<CreateConnectionCommand>,CreateConnectionCommandHandler>();
        services.AddSingleton<IHandler<DeleteConnectionCommand>,DeleteConnectionCommandHandler>();
        
        services.AddSingleton<IHandler<CreateClientCommand>,CreateClientCommandHandler>();
        services.AddSingleton<IHandler<DeleteClientCommand>,DeleteClientCommandHandler>();
        services.AddSingleton<IHandler<GetClientCommand>,GetClientCommandHandler>();
        services.AddSingleton<IHandler<ChangeClientNameCommand>,ChangeClientNameCommandHandler>();
        services.AddSingleton<IHandler<ChangeClientAuthTokenCommand>,ChangeClientAuthTokenCommandHandler>();

        services.AddSingleton<IHandler<ConnectionCreatedDomainEvent>, ConnectionCreatedEventHandler>();
        services.AddSingleton<IHandler<ConnectionDeletedDomainEvent>, ConnectionDeletedEventHandler>();
        services.AddSingleton<IHandler<ClientCreatedDomainEvent>, ClientCreatedEventHandler>();
        services.AddSingleton<IHandler<ClientDeletedDomainEvent>, ClientDeletedEventHandler>();
        services.AddSingleton<IHandler<ClientNameChangedDomainEvent>, ClientNameChangedEventHandler>();
        services.AddSingleton<IHandler<ClientAuthTokenChangedDomainEvent>, ClientAuthTokenChangedEventHandler>();
    }
}