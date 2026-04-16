using Dara.BuildingBlocks.Domain.Commands;
using Dara.BuildingBlocks.Domain.Events;
using Dara.Core.Infrastructure;

namespace Dara.Apps.Server.API.Initializers;

public static class CoreInitializer
{
    public static void AddBuildingBlocks(this IServiceCollection services)
    {
        services.AddSingleton<IApplicationCommandDispatcher, ApplicationCommandDispatcher>();
        services.AddSingleton<IDomainEventDispatcher, DomainEventDispatcher>();
    }
}