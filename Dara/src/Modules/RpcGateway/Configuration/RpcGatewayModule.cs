using Dara.Modules.RpcGateway.Infrastructure;
using Microsoft.AspNetCore.Routing;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Dara.BuildingBlocks.Configuration;
using Dara.Modules.RpcGateway.Application;
using Dara.Modules.RpcGateway.Infrastructure.Domain;

namespace Dara.Modules.RpcGateway.Configuration;

public static class RpcGatewayModule
{
    public static IServiceCollection AddRpcGatewayModule(this IServiceCollection services)
    {
        //signalR (Hubs Logic)
        services.AddSignalR();
        
        //commands
        services.AddCommandHandler<GetIpConnectionsCommandHandler>();
        services.AddCommandHandler<SetConnectionEstablishedCommandHandler>();
        services.AddCommandHandler<SetConnectionLostCommandHandler>();
        
        //events
        services.AddDomainEventHandler<ConnectionEstablishedEventHandler>();
        services.AddDomainEventHandler<ConnectionLostEventHandler>();
        
        //repository
        services.AddRepository<ConnectionRepository>();
        
        return services;
    }
    
    public static IEndpointRouteBuilder UseGatewayModule(this IEndpointRouteBuilder endpoints)
    {
        //add app hub endpoint
        endpoints.MapHub<AppHub>("/app");
        
        return endpoints;
    }
}