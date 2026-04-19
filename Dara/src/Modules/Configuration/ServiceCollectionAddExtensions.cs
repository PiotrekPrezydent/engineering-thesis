using Dara.BuildingBlocks.Application;
using Dara.BuildingBlocks.Domain;
using Dara.BuildingBlocks.Infrastructure;
using Microsoft.Extensions.DependencyInjection;

namespace Dara.Modules.Configuration;

public static class ServiceCollectionAddExtensions
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