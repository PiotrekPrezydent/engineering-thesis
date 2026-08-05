using System.Reflection;

namespace Dara.Server.BuildingBlocks.Infrastructure.Extensions;

public static class AssemblyExtensions
{
    public static IEnumerable<(Type Interface, Type Implementation)> GetImplementationsOfOpenGeneric(this Assembly assembly, Type openGenericType)
    {
        return assembly.GetTypes()
            .Where(t => t.IsClass && !t.IsAbstract)
            .SelectMany(t => t.GetInterfaces()
                .Where(i => i.IsGenericType && i.GetGenericTypeDefinition() == openGenericType)
                .Select(i => (Interface: i, Implementation: t)));
    }

    public static (Type Interface, Type Implementation) GetFirstImplementationOfType(this Assembly assembly, Type interfaceType)
    {
        var candidate = assembly.GetTypes().Where(t=>t.IsClass && !t.IsAbstract && interfaceType.IsAssignableFrom(t)).FirstOrDefault();
        if(candidate == null)
            throw new ArgumentException($"could not find implementation of type {interfaceType.Name}");
        
        return (interfaceType, candidate);
    }
    
}