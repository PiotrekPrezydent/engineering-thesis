using Dara.BuildingBlocks.Application.Abstraction;
using Dara.BuildingBlocks.Domain.Events.Abstractions;
using Dara.BuildingBlocks.Infrastructure;
using Microsoft.Extensions.DependencyInjection;

namespace Dara.Modules.Configuration;

public static class BuildingBlocks
{
    public static IServiceCollection AddBuildingBlocksInfraDispatchers(this IServiceCollection services)
    {
        services.AddSingleton<IApplicationCommandDispatcher, ApplicationCommandDispatcher>();
        services.AddSingleton<IDomainEventDispatcher, DomainEventDispatcher>();
        
        return services;
    }
}