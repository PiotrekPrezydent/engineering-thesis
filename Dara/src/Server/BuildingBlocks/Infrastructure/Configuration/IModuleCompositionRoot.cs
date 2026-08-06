using Microsoft.Extensions.DependencyInjection;

namespace Dara.Server.BuildingBlocks.Infrastructure.Configuration;

public interface IModuleCompositionRoot
{
    public void Initialize(IServiceCollection rootServices);
    public IServiceScope CreateScope();
}

