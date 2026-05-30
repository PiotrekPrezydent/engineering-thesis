using Dara.BuildingBlocks.Infrastructure.Abstractions;
using Microsoft.Extensions.DependencyInjection;

namespace Dara.BuildingBlocks.Infrastructure.Configuration;

public class BuildingBlocksCompositionRoot
{
    public static void Initialize(IServiceCollection services)
    {
        services.AddSingleton<BuildingBlocksLogger>();
        
        services.AddSingleton<IDomainEventDispatcher, DomainEventDispatcher>();
        services.AddSingleton<IModuleCommandRunner, ModuleCommandRunner>();
        services.AddSingleton<IIntegrationEventDispatcher, IntegrationEventDispatcher>();
    }
}