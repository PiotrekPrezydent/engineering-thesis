using Microsoft.Extensions.DependencyInjection;

namespace Dara.BuildingBlocks.Infrastructure.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddDecorators(this IServiceCollection services, IEnumerable<Type> decorators)
    {
        foreach (var decorator in decorators)
            services.UseDecorator(decorator); 

        return services;
    }
    public static IServiceCollection UseDecorator(this IServiceCollection services, Type decoratorType)
    {
        var descriptor = services.LastOrDefault();
        
        if (descriptor == null)
            throw new InvalidOperationException("No service registered for " + decoratorType.Name);

        var serviceType = descriptor.ServiceType; 
        Type closedDecoratorType;
        
        if (decoratorType.IsGenericTypeDefinition)
        {
            if (!serviceType.IsGenericType)
                throw new InvalidOperationException($"Service {serviceType.Name} is not generic type, but {decoratorType.Name} is generic.");
            
            var genericArguments = serviceType.GetGenericArguments();
            closedDecoratorType = decoratorType.MakeGenericType(genericArguments);
        }
        else
            closedDecoratorType = decoratorType;
        
        services.Remove(descriptor);

        services.Add(new ServiceDescriptor(serviceType, provider =>
        {
            object innerInstance;
            if (descriptor.ImplementationInstance != null)
                innerInstance = descriptor.ImplementationInstance;
            else if (descriptor.ImplementationFactory != null)
                innerInstance = descriptor.ImplementationFactory(provider);
            else
                innerInstance = ActivatorUtilities.CreateInstance(provider, descriptor.ImplementationType!);

            return ActivatorUtilities.CreateInstance(provider, closedDecoratorType, innerInstance);

        }, descriptor.Lifetime));

        return services;
    }
}