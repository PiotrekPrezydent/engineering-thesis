using Dara.BuildingBlocks.Domain;
using Dara.BuildingBlocks.Infrastructure.Extensions;
using Dara.BuildingBlocks.Infrastructure.Processing;
using Dara.Server.BuildingBlocks.Application;
using Dara.Server.BuildingBlocks.Application.Commands;
using Dara.Server.BuildingBlocks.Application.Events;
using Microsoft.Extensions.DependencyInjection;

namespace Dara.BuildingBlocks.Infrastructure;

public abstract class BaseCompositionRoot<TBuilder> : ICompositionRoot where TBuilder : BaseCompositionRoot<TBuilder>, new()
{
    protected static IServiceProvider? _rootProvider;
    protected abstract Type ApplicationType { get; }

    protected virtual IReadOnlyList<Type> GlobalDecorators => Array.Empty<Type>();
    protected virtual IReadOnlyList<(Type ImplementationType, Type DecoratorType)> TypeInclusiveDecorators => Array.Empty<(Type, Type)>();
    protected virtual IReadOnlyList<(Type ImplementationType, Type DecoratorType)> TypeExclusiveDecorators => Array.Empty<(Type, Type)>();

    public static void Initialize()
    {
        var services = new ServiceCollection();
        var builder = new TBuilder();
        var infrastructureAssembly = typeof(TBuilder).Assembly;
        var applicationAssembly = builder.ApplicationType.Assembly;
        
        AddScopedServiceWithDecorators(typeof(IUnitOfWork), typeof(UnitOfWork));
        AddScopedServiceWithDecorators(typeof(IDomainEventsDispatcher), typeof(DomainEventsDispatcher));
        AddScopedServiceWithDecorators(typeof(IHandlersResolver), typeof(HandlersResolver));
        AddScopedServiceWithDecorators(typeof(ICommandExecutor), typeof(CommandExecutor));
        
        FindAndAddTransientHandlersWithTypeWithDecorators(typeof(ICommandHandler<>));
        FindAndAddTransientHandlersWithTypeWithDecorators(typeof(ICommandHandler<,>));
        FindAndAddTransientHandlersWithTypeWithDecorators(typeof(IDomainEventHandler<>));
        
        //this will find decorators as well as repositories becouse they both inheritance from IRepository in interfaces
        var repos = infrastructureAssembly.GetTypes()
            .Where(t => t.IsClass && !t.IsAbstract)
            .SelectMany(t => t.GetInterfaces()
                .Where(i => typeof(IRepository).IsAssignableFrom(i) && i != typeof(IRepository))
                .Select(i => (Interface: i, Implementation: t)));

        foreach (var repo in repos)
            AddScopedServiceWithDecorators(repo.Interface, repo.Implementation);
        
        builder.AddModuleContext(services);

        _rootProvider = services.BuildServiceProvider();
        return;
        
        void AddScopedServiceWithDecorators(Type serviceType, Type implementationType)
        {
            services.AddScoped(serviceType, implementationType);
            services.AddDecorators(builder.GetDecoratorsToBeAdded(serviceType, implementationType));
        }

        void FindAndAddTransientHandlersWithTypeWithDecorators(Type handlerType)
        {
            var handlers = applicationAssembly.GetImplementationsOfOpenGeneric(handlerType);
            foreach (var handler in handlers)
            {
                services.AddTransient(handler.Interface, handler.Implementation);
                services.AddDecorators(builder.GetDecoratorsToBeAdded(handler.Interface, handler.Implementation));
            }
        }
    }
    
    public static IServiceScope CreteScope()
    {
        if (_rootProvider == null)
            throw new InvalidOperationException($"CompositionRoot for {typeof(TBuilder).Name} was not initialized.");
        
        return _rootProvider.CreateScope();
    }
    
    protected abstract IServiceCollection AddModuleContext(IServiceCollection services);
    
    protected IEnumerable<Type> GetDecoratorsToBeAdded(Type serviceInterface, Type implementationType)
    {
        Type targetInterface = serviceInterface.IsGenericType 
            ? serviceInterface.GetGenericTypeDefinition() 
            : serviceInterface;
        
        var globalDecorators = this.GlobalDecorators.Where(decoratorType => 
        {
            if (targetInterface.IsGenericTypeDefinition)
            {
                return decoratorType.GetInterfaces().Any(i => 
                    i.IsGenericType && 
                    i.GetGenericTypeDefinition() == targetInterface);
            }
            return targetInterface.IsAssignableFrom(decoratorType);
        });
        
        var inclusiveDecorators = this.TypeInclusiveDecorators
            .Where(x => x.ImplementationType == implementationType)
            .Select(x => x.DecoratorType);
        
        var exclusiveDecorators = this.TypeExclusiveDecorators
            .Where(x => x.ImplementationType == implementationType)
            .Select(x => x.DecoratorType)
            .ToHashSet();
        
        foreach (var decorator in inclusiveDecorators)
            if (!exclusiveDecorators.Contains(decorator))
                yield return decorator;

        foreach (var decorator in globalDecorators)
            if (!exclusiveDecorators.Contains(decorator))
                yield return decorator;
    }
}