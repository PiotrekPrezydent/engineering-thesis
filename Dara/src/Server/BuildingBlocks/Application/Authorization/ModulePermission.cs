namespace Dara.Server.BuildingBlocks.Application.Authorization;

// base of the permission type of a module, one per module:
// sealed record GroupsPermission(GroupsPermission.Permissions Value) : ModulePermission<GroupsPermission.Permissions>(Value)
public abstract record ModulePermission<TPermissions>(TPermissions Value) : IModulePermission where TPermissions : struct, Enum
{
    public string PermissionCode => $"{GetType().Name}.{Value}";
}
