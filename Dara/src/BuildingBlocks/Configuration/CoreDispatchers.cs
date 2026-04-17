using Dara.BuildingBlocks.Domain.Commands;
using Dara.BuildingBlocks.Domain.Events;
using Dara.BuildingBlocks.Infrastructure;
using Microsoft.Extensions.DependencyInjection;

namespace Dara.BuildingBlocks.Configuration;

public static class CoreDispatchers
{
    public static IServiceCollection AddCoreDispatchers(this IServiceCollection services)
    {
        services.AddSingleton<IApplicationCommandDispatcher, ApplicationCommandDispatcher>();
        services.AddSingleton<IDomainEventDispatcher, DomainEventDispatcher>();
        
        return services;
    }
}