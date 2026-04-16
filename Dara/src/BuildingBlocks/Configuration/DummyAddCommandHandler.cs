using Dara.BuildingBlocks.Application;
using Microsoft.Extensions.DependencyInjection;

namespace Dara.BuildingBlocks.Configuration;

public static class DummyAddCommandHandler
{
    public static IServiceCollection AddCommandHandler<THandler>(this IServiceCollection services)
        where THandler : class
    {
        var handlerType = typeof(THandler);
        
        var interfaceType = handlerType.GetInterfaces().FirstOrDefault(i => 
            i.IsGenericType && 
            i.GetGenericTypeDefinition() == typeof(IApplicationCommandHandler<,>));

        if (interfaceType == null)
        {
            throw new ArgumentException($"Type {handlerType.Name} do not implement IApplicationCommandHandler<,>!");
        }
        
        return services.AddSingleton(interfaceType, handlerType);
    }
}