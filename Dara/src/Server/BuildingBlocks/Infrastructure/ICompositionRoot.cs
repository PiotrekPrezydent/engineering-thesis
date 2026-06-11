using Microsoft.Extensions.DependencyInjection;

namespace Dara.Server.BuildingBlocks.Infrastructure;

public interface ICompositionRoot
{
    public static abstract void Initialize();

    public static abstract IServiceScope CreteScope();
}