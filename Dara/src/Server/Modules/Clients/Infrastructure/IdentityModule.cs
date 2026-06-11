using Dara.Server.Modules.Clients.Application.Clients.ChangeClientName;
using Dara.Server.Modules.Clients.Application.Clients.EndClientSession;
using Dara.Server.Modules.Clients.Application.Clients.StartClientSession;
using Dara.Server.Modules.Clients.Domain.Clients;
using Microsoft.Extensions.DependencyInjection;

namespace Dara.Server.Modules.Clients.Infrastructure;

public static class IdentityModule
{
    public static IServiceCollection AddIdentityModule(this IServiceCollection services)
    {
        services.AddEasyImplementation<IClientRepository>();
        
        services.AddEasyImplementation<ChangeClientNameCommandCommandHandler>();
        services.AddEasyImplementation<EndClientSessionCommandCommandHandler>();
        services.AddEasyImplementation<StartClientSessionCommandCommandHandler>();
        
        services.AddEasyImplementation<ClientChangedNameDomainEventCommandHandler>();
        services.AddEasyImplementation<ClientEndedSessionDomainEventCommandHandler>();
        services.AddEasyImplementation<ClientStartedSessionDomainEventCommandHandler>();
        
        return services;
    }
}