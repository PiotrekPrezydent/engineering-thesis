using Microsoft.Extensions.DependencyInjection;

namespace Dara.BuildingBlocks.Infrastructure.CompositionRoots;

public static class EasyImplementationRegister
{
    public static void AddEasyImplementation<T>(this IServiceCollection services)
    {
        var interfaceType = typeof(T).GetInterfaces().First();
        services.AddSingleton(interfaceType, typeof(T));
    }
}