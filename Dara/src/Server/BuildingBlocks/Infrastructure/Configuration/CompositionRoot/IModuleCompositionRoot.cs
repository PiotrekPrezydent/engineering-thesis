using Microsoft.Extensions.DependencyInjection;

namespace Dara.Server.BuildingBlocks.Infrastructure.Configuration.CompositionRoot;

public interface IModuleCompositionRoot
{
    public void Initialize(IServiceCollection rootServices);
    public IServiceScope CreateScope();
}

