using Dara.BuildingBlocks.Domain.Commands;
using Dara.BuildingBlocks.Domain.Events;
using Dara.Modules.RpcGateway.Application.Contracts;
using Dara.Modules.RpcGateway.Application.Domain;
using Dara.Modules.RpcGateway.Contracts;
using Dara.Modules.RpcGateway.Domain;
using Dara.Modules.RpcGateway.Domain.Events;
using Dara.Modules.RpcGateway.Infrastructure;
using Microsoft.AspNetCore.Routing;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace Dara.Modules.RpcGateway.Configuration;

public static class RpcGatewayModule
{
    public static IServiceCollection AddRpcGatewayModule(this IServiceCollection services)
    {
        //signalR (HubsLogic)
        services.AddSignalR();

        //commands
        services.AddSingleton<IApplicationCommandHandler<ChangeClientAuthTokenCommand,ChangeClientAuthTokenCommandResult>, ChangeClientAuthTokenCommandHandler>();
        services.AddSingleton<IApplicationCommandHandler<ChangeClientNameCommand, ChangeClientNameCommandResult>, ChangeClientNameCommandHandler>();
        services.AddSingleton<IApplicationCommandHandler<ClientConnectedCommand, ClientConnectedCommandResult>, ClientConnectedCommandHandler>();
        services.AddSingleton<IApplicationCommandHandler<ClientDisconnectedCommand, ClientDisconnectedCommandResult>,ClientDisconnectedCommandHandler>();
        
        //events
        services.AddSingleton<IDomainEventHandler<ClientConnectionCreatedEvent>, ClientConnectionCreatedEventHandler>();
        services.AddSingleton<IDomainEventHandler<ClientConnectionRemovedEvent>, ClientConnectionRemovedEventHandler>();
        
        //repository
        services.AddSingleton<IClientConnectionRepository, ClientConnectionRepository>();
        
        return services;
    }
    
    public static IEndpointRouteBuilder UseGatewayModule(this IEndpointRouteBuilder endpoints)
    {
        //add app hub endpoint
        endpoints.MapHub<AppHub>("/app");
        
        return endpoints;
    }
}