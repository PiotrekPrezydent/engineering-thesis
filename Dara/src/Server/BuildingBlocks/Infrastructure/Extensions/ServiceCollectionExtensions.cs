using System.Reflection;
using Microsoft.Extensions.DependencyInjection;

namespace Dara.Server.BuildingBlocks.Infrastructure.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddAssemblyInterfaceImplementations(this IServiceCollection services, Assembly assembly, Type interfaceType, ServiceLifetime serviceLifetime)
    {
        if (interfaceType.IsGenericType)
        {
            var openGenericHandlers = assembly.GetImplementationsOfOpenGeneric(interfaceType);

            foreach (var handler in openGenericHandlers)
                services.Add(ServiceDescriptor.Describe(handler.Interface, handler.Implementation, serviceLifetime));
        }
        else
        {
            var implementations = assembly.GetTypes()
                .Where(t => t.IsClass && !t.IsAbstract && interfaceType.IsAssignableFrom(t));

            foreach (var implementation in implementations)
                services.Add(ServiceDescriptor.Describe(interfaceType, implementation, serviceLifetime));
        }
        
        return services;
    }
}