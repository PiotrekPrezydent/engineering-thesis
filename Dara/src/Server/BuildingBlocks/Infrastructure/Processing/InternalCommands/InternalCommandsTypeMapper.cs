using System.Reflection;
using Dara.Server.BuildingBlocks.Application.Commands;
using Dara.Server.BuildingBlocks.Infrastructure.Common;

namespace Dara.Server.BuildingBlocks.Infrastructure.Processing.InternalCommands;

public class InternalCommandsTypeMapper : IInternalCommandsTypeMapper
{
    private readonly BiDictionary<Type, string> _typesNamesMap;

    public InternalCommandsTypeMapper(BiDictionary<Type, string> typesNamesMap)
    {
        _typesNamesMap = typesNamesMap;
    }

    // every concrete command without a result of the assembly, by class name;
    // a repeated class name throws at the start of the module, like the outbox map
    public static InternalCommandsTypeMapper FromAssembly(Assembly applicationAssembly)
    {
        var map = new BiDictionary<Type, string>();

        var commandTypes = applicationAssembly.GetTypes()
            .Where(t => t.IsClass && !t.IsAbstract && !t.IsGenericTypeDefinition)
            .Where(t => typeof(ICommand).IsAssignableFrom(t))
            .Where(t => !t.GetInterfaces().Any(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(ICommand<>)));

        foreach (var commandType in commandTypes)
            map.Add(commandType, commandType.Name);

        return new InternalCommandsTypeMapper(map);
    }

    public string GetTypeNameForCommandWithType(Type type)
    {
        if (!_typesNamesMap.TryGetByFirst(type, out var name))
            throw new InvalidOperationException($"{type.FullName} cannot be queued as an internal command: it is not a command without a result of this module.");

        return name;
    }

    public Type GetTypeForCommandWithTypeName(string typeName)
    {
        if (!_typesNamesMap.TryGetBySecond(typeName, out var type))
            throw new InvalidOperationException($"No internal command type with the name {typeName} in this module.");

        return type;
    }
}
