using System.Reflection;

namespace Dara.BuildingBlocks.Infrastructure.Extensions;

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
}