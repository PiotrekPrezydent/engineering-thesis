using Dara.Server.BuildingBlocks.Infrastructure.Messaging.EventBus;
using Microsoft.Extensions.DependencyInjection;

namespace Dara.Server.BuildingBlocks.Infrastructure.Configuration;

public interface IModuleCompositionRoot
{
    public void Initialize(IServiceCollection rootServices, IEventBus eventBus);
    public IServiceScope CreateScope();
}

