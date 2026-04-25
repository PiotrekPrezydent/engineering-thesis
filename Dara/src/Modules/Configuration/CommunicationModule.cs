using Dara.Modules.Communication.Application.Clients.ChangeClientAuthToken;
using Dara.Modules.Communication.Application.Clients.ChangeClientName;
using Dara.Modules.Communication.Application.Clients.CreateClient;
using Dara.Modules.Communication.Application.Clients.DeleteClient;
using Dara.Modules.Communication.Application.Clients.GetClient;
using Dara.Modules.Communication.Application.Connections.CreateConnection;
using Dara.Modules.Communication.Application.Connections.DeleteConnection;
using Dara.Modules.Communication.Domain.Clients;
using Dara.Modules.Communication.Domain.Connections;
using Dara.Modules.Communication.Infrastructure;
using Microsoft.Extensions.DependencyInjection;

namespace Dara.Modules.Configuration;

public static class CommunicationModule
{
    public static IServiceCollection AddCommunicationModule(this IServiceCollection services)
    {
        //repository
        services.AddSingleton<IConnectionRepository,ConnectionRepository>();
        services.AddSingleton<IClientRepository, ClientRepository>();
        
        //commands
        services.AddCommandHandler<CreateConnectionCommandHandler>();
        services.AddCommandHandler<DeleteConnectionCommandHandler>();
        
        services.AddCommandHandler<CreateClientCommandHandler>();
        services.AddCommandHandler<DeleteClientCommandHandler>();
        services.AddCommandHandler<GetClientCommandHandler>();
        services.AddCommandHandler<ChangeClientNameCommandHandler>();
        services.AddCommandHandler<ChangeClientAuthTokenCommandHandler>();
        
        
        //events
        services.AddDomainEventHandler<ConnectionCreatedEventHandler>();
        services.AddDomainEventHandler<ConnectionDeletedEventHandler>();
        
        services.AddDomainEventHandler<ClientCreatedEventHandler>();
        services.AddDomainEventHandler<ClientDeletedEventHandler>();
        services.AddDomainEventHandler<ClientNameChangedEventHandler>();
        services.AddDomainEventHandler<ClientAuthTokenChangedEventHandler>();
        
        return services;
    } 
}