using Dara.BuildingBlocks.Infrastructure.CompositionRoots;
using Dara.Server.Modules.Identity.Application.Clients.AddClientPlugin;
using Dara.Server.Modules.Identity.Application.Clients.AddClientSupervisor;
using Dara.Server.Modules.Identity.Application.Clients.ChangeClientName;
using Dara.Server.Modules.Identity.Application.Clients.EndClientSession;
using Dara.Server.Modules.Identity.Application.Clients.RemoveClientPlugin;
using Dara.Server.Modules.Identity.Application.Clients.RemoveClientSupervisor;
using Dara.Server.Modules.Identity.Application.Clients.StartClientSession;
using Dara.Server.Modules.Identity.Domain.Clients;
using Microsoft.Extensions.DependencyInjection;

namespace Dara.Server.Modules.Identity.Infrastructure;

public static class IdentityModule
{
    public static IServiceCollection AddIdentityModule(this IServiceCollection services)
    {
        services.AddEasyImplementation<IClientRepository>();
        services.AddEasyImplementation<JsonClientPluginParser>();
        
        services.AddEasyImplementation<AddClientPluginCommandHandler>();
        services.AddEasyImplementation<AddClientSupervisorCommandHandler>();
        services.AddEasyImplementation<ChangeClientNameCommandHandler>();
        services.AddEasyImplementation<EndClientSessionCommandHandler>();
        services.AddEasyImplementation<RemoveClientPluginCommandHandler>();
        services.AddEasyImplementation<RemoveClientSupervisorCommandHandler>();
        services.AddEasyImplementation<StartClientSessionCommandHandler>();
        
        services.AddEasyImplementation<ClientAddedPluginDomainEventHandler>();
        services.AddEasyImplementation<ClientAddedSupervisorDomainEventHandler>();
        services.AddEasyImplementation<ClientChangedNameDomainEventHandler>();
        services.AddEasyImplementation<ClientEndedSessionDomainEventHandler>();
        services.AddEasyImplementation<ClientRemovedPluginDomainEventHandler>();
        services.AddEasyImplementation<ClientRemovedSupervisorDomainEventHandler>();
        services.AddEasyImplementation<ClientStartedSessionDomainEventHandler>();
        
        return services;
    }
}