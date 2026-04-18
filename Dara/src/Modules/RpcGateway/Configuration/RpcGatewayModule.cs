using Dara.Modules.RpcGateway.Infrastructure;
using Microsoft.AspNetCore.Routing;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Dara.BuildingBlocks.Configuration;

namespace Dara.Modules.RpcGateway.Configuration;

public static class RpcGatewayModule
{
    public static IServiceCollection AddRpcGatewayModule(this IServiceCollection services)
    {
        // //signalR (HubsLogic)
        // services.AddSignalR();
        //
        // //commands
        // services.AddCommandHandler<ChangeClientAuthTokenCommandHandler>();
        // services.AddCommandHandler<ChangeClientNameCommandHandler>();
        // services.AddCommandHandler<ClientConnectedCommandHandler>();
        // services.AddCommandHandler<ClientDisconnectedCommandHandler>();
        //
        // //events
        // services.AddDomainEventHandler<ClientConnectionCreatedEventHandler>();
        // services.AddDomainEventHandler<ClientConnectionRemovedEventHandler>();
        //
        // //repository
        // services.AddRepository<ClientConnectionRepository>();
        // //services.AddSingleton<IClientConnectionRepository, ClientConnectionRepository>();
        
        return services;
    }
    
    public static IEndpointRouteBuilder UseGatewayModule(this IEndpointRouteBuilder endpoints)
    {
        //add app hub endpoint
        endpoints.MapHub<AppHub>("/app");
        
        return endpoints;
    }
}