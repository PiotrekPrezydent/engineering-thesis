using Dara.BuildingBlocks.Application;
using Dara.BuildingBlocks.Domain;
using Microsoft.Extensions.DependencyInjection;

namespace Dara.BuildingBlocks.Configuration;

public static class DummyAddRepository
{
    public static IServiceCollection AddRepository<TRepository>(this IServiceCollection services)
        where TRepository : class
    {
        var handlerType = typeof(TRepository);
        
        var interfaceType = handlerType.GetInterfaces().FirstOrDefault(j => 
            j.GetInterfaces().Any( i=>
                i.IsGenericType && 
                i.GetGenericTypeDefinition() == typeof(IRepository<>)));

        if (interfaceType == null)
        {
            throw new ArgumentException($"Type {handlerType.Name} do not implement IRepository<>!");
        }
        
        return services.AddSingleton(interfaceType, handlerType);
    }
}