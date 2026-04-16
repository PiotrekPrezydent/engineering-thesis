using Dara.Modules.RpcGateway.Infrastructure;
using Microsoft.AspNetCore.Routing;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace Dara.Modules.RpcGateway.Configuration;

public static class RpcGatewayModule
{
    public static IServiceCollection AddRpcGatewayModule(this IServiceCollection services)
    {
        services.AddSignalR();
        //Add commands etc.
        
        return services;
    }
    
    public static IEndpointRouteBuilder UseGatewayModule(this IEndpointRouteBuilder endpoints)
    {
        // Wystawiamy nasz RpcHub pod konkretnym adresem URL
        endpoints.MapHub<AppHub>("/app");
        
        return endpoints;
    }
}