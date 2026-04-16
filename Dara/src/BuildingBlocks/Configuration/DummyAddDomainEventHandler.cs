using Dara.BuildingBlocks.Application;
using Microsoft.Extensions.DependencyInjection;

namespace Dara.BuildingBlocks.Configuration;

public static class DummyAddDomainEventHandler
{
    public static IServiceCollection AddDomainEventHandler<THandler>(this IServiceCollection services)
        where THandler : class
    {
        var handlerType = typeof(THandler);
        
        var interfaceType = handlerType.GetInterfaces().FirstOrDefault(i => 
            i.IsGenericType && 
            i.GetGenericTypeDefinition() == typeof(IDomainEventHandler<>));

        if (interfaceType == null)
        {
            throw new ArgumentException($"Type {handlerType.Name} do not implement IDomainEventHandler<>!");
        }
        
        return services.AddSingleton(interfaceType, handlerType);
    }
}