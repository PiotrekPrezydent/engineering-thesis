using Dara.BuildingBlocks.Application;
using Dara.BuildingBlocks.Application.Commands;
using Dara.BuildingBlocks.Application.Domain;
using Dara.BuildingBlocks.Application.Integration;
using Dara.BuildingBlocks.Infrastructure.Integration;
using Microsoft.Extensions.DependencyInjection;

namespace Dara.BuildingBlocks.Infrastructure;

public class Module
{
    internal IApplicationLayer AppLayer;
    internal IInfrastructureLayer InfraLayer;

    public Module(IApplicationLayer appLayer, IInfrastructureLayer infraLayer)
    {
        AppLayer = appLayer;
        InfraLayer = infraLayer;
    }
}

public static class ServicesExtensions
{
    public static IServiceCollection AddModule(this IServiceCollection services, Module module)
    {
        AddApplicationLayer(services, module.AppLayer);
        AddInfrastructureLayer(services, module.InfraLayer);
        
        return services;
    }
    
    static IServiceCollection AddApplicationLayer(IServiceCollection services, IApplicationLayer layer)
    {
        foreach (var commandHandler in layer.GetModuleCommandHandlers)
        {
            Type? interfaceType = commandHandler.GetInterfaces().FirstOrDefault(i => 
                i.IsGenericType && 
                i.GetGenericTypeDefinition() == typeof(IModuleCommandHandler<,>));
            
            if (interfaceType == null)
            {
                throw new ArgumentException($"Type {commandHandler.Name} do not implement IApplicationCommandHandler<,>!");
            }
            
            services.AddSingleton(interfaceType, commandHandler);
        }

        foreach (var domainEventHandler in layer.GetDomainEventHandlers)
        {
            Type? interfaceType = domainEventHandler.GetInterfaces().FirstOrDefault(i => 
                i.IsGenericType && 
                i.GetGenericTypeDefinition() == typeof(IDomainEventHandler<>));
            
            if (interfaceType == null)
            {
                throw new ArgumentException($"Type {domainEventHandler.Name} do not implement IDomainEventHandler<>!");
            }
            
            services.AddSingleton(interfaceType, domainEventHandler);
        }

        foreach (var integrationEventHandler in layer.GetIntegrationEventHandlers)
        {
            Type handlerType = integrationEventHandler.GetType();
            
            Type? interfaceType = handlerType.GetInterfaces().FirstOrDefault(i => 
                i.IsGenericType && 
                i.GetGenericTypeDefinition() == typeof(IIntegrationEventHandler<>));
            
            if (interfaceType == null)
            {
                throw new ArgumentException($"Type {handlerType.Name} do not implement IIntegrationEventHandler<>!");
            }

            Type eventTypeT = interfaceType.GetGenericArguments()[0];
            
            IntegrationEventBus.Instance.Subscribe(eventTypeT, integrationEventHandler);
        }
        
        return services;
    }

    static IServiceCollection AddInfrastructureLayer(this IServiceCollection services, IInfrastructureLayer layer)
    {
        foreach (var repository in layer.GetRepositoriesImplementations)
        {
            Type? interfaceType = repository.GetInterfaces().First();
            
            if (interfaceType == null)
            {
                throw new ArgumentException($"Type {repository.Name} do not implement any IRepository!");
            }
            
            services.AddSingleton(interfaceType, repository);
        }
        
        return services;
    }
        
}