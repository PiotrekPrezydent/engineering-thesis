using Dara.Modules.Communication.Application.Clients;
using Dara.Modules.Communication.Application.Nodes;
using Dara.Modules.Communication.Application.NodesMeshes;
using Dara.Modules.Communication.Infrastructure;
using Microsoft.Extensions.DependencyInjection;

namespace Dara.Modules.Configuration;

public static class CommunicationModule
{
    public static IServiceCollection AddCommunicationModule(this IServiceCollection services)
    {
        //repository
        services.AddRepository<ClientsRepository>();
        services.AddRepository<NodesRepository>();
        services.AddRepository<NodesMeshesRepository>();
        
        //commands
        services.AddCommandHandler<CreateClientCommandHandler>();
        services.AddCommandHandler<DeleteClientCommandHandler>();
        services.AddCommandHandler<GetClientCommandHandler>();
        services.AddCommandHandler<GetClientsCommandHandler>();

        services.AddCommandHandler<CreateNodeCommandHandler>();
        services.AddCommandHandler<DeleteNodeCommandHandler>();
        services.AddCommandHandler<GetNodeCommandHandler>();
        services.AddCommandHandler<GetNodesCommandHandler>();
        services.AddCommandHandler<ChangeNodeNameCommandHandler>();
        services.AddCommandHandler<ChangeNodeAuthTokenCommandHandler>();

        services.AddCommandHandler<CreateNodesMeshCommandHandler>();
        services.AddCommandHandler<DeleteNodesMeshCommandHandler>();
        services.AddCommandHandler<GetNodesMeshCommandHandler>();
        services.AddCommandHandler<GetNodesMeshesCommandHandler>();
        services.AddCommandHandler<ChangeNodesMeshNameCommandHandler>();
        services.AddCommandHandler<ChangeNodesMeshCodeCommandHandler>();
        services.AddCommandHandler<AddNodeToNodesMeshCommandHandler>();
        services.AddCommandHandler<RemoveNodeFromNodesMeshCommandHandler>();
 
        
        //events
        // services.AddDomainEventHandler<ConnectionEstablishedEventHandler>();
        // services.AddDomainEventHandler<ConnectionLostEventHandler>();
        
        return services;
    } 
}