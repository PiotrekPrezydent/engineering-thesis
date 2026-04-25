using Dara.BuildingBlocks.Infrastructure.Commands;
using Dara.BuildingBlocks.Infrastructure.Domain;
using Microsoft.Extensions.DependencyInjection;

namespace Dara.Modules.Configuration
{
    public static class BuildingBlocks
    {
        public static IServiceCollection AddBuildingBlocksInfraDispatchers(this IServiceCollection services)
        {
            services.AddSingleton<IApplicationCommandDispatcher, ApplicationCommandDispatcher>();
            services.AddSingleton<IDomainEventDispatcher, DomainEventDispatcher>();
        
            return services;
        }
    }
}