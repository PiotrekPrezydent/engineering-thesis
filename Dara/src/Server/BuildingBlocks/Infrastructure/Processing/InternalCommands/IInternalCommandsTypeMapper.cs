namespace Dara.Server.BuildingBlocks.Infrastructure.Processing.InternalCommands;

public interface IInternalCommandsTypeMapper
{
    // throws for a type that cannot be queued (not a command of the module, or a command with a result)
    string GetTypeNameForCommandWithType(Type type);

    Type GetTypeForCommandWithTypeName(string typeName);
}
