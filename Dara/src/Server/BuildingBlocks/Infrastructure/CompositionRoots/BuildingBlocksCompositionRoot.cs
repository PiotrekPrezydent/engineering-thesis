using Dara.BuildingBlocks.Infrastructure.DomainEvents;
using Dara.BuildingBlocks.Infrastructure.IntegrationEvents;
using Dara.BuildingBlocks.Infrastructure.ModuleCommands;
using Microsoft.Extensions.DependencyInjection;

namespace Dara.BuildingBlocks.Infrastructure.CompositionRoots;

public class BuildingBlocksCompositionRoot
{
    public static void Initialize(IServiceCollection services)
    {
        services.AddEasyImplementation<DomainEventDispatcher>();
        services.AddEasyImplementation<ModuleCommandRunner>();
        services.AddEasyImplementation<IntegrationEventDispatcher>();
    }
}