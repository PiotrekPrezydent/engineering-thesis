using Microsoft.Extensions.DependencyInjection;

namespace Dara.Server.BuildingBlocks.Infrastructure.Extensions;

public static class DecoratorExtensions
{
    public static IServiceCollection AddTypeWiseDecorator(this IServiceCollection services, Type decoratorType)
    {
        var decoratorInterface = decoratorType.GetInterfaces().FirstOrDefault();
        if(decoratorInterface == null)
            throw new Exception($"Decorator is not implementing any interface");
        
        IEnumerable<ServiceDescriptor> descriptorsToChange;

        if (decoratorInterface.IsGenericType)
        {
            decoratorInterface = decoratorInterface.GetGenericTypeDefinition();
            descriptorsToChange = services.Where(e => e.ServiceType.IsGenericType && e.ServiceType.GetGenericTypeDefinition() == decoratorInterface);
        }
        else
        {
            descriptorsToChange = services.Where(e => e.ServiceType == decoratorType);
        }
        
        
        foreach (var descriptor in descriptorsToChange.ToList())
        {
            var usedDecoratorType = decoratorType;
            
            if(decoratorInterface.IsGenericType)
                usedDecoratorType = usedDecoratorType.MakeGenericType(descriptor.ServiceType.GetGenericArguments());

            var newDescriptor = new ServiceDescriptor(descriptor.ServiceType, sp =>
            {
                object innerInstance;
                if (descriptor.ImplementationInstance != null)
                    innerInstance = descriptor.ImplementationInstance;
                else if (descriptor.ImplementationFactory != null)
                    innerInstance = descriptor.ImplementationFactory(sp);
                else
                    innerInstance = ActivatorUtilities.CreateInstance(sp, descriptor.ImplementationType!);
                
                return ActivatorUtilities.CreateInstance(sp, usedDecoratorType, innerInstance);
            }, descriptor.Lifetime);

            services.Remove(descriptor);
            services.Add(newDescriptor);
        }

        return services;
    }
}