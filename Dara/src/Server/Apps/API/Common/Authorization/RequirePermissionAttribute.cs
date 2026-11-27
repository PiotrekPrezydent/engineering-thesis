using System.Reflection;
using Dara.Server.BuildingBlocks.Application.Authorization;

namespace Dara.Server.Apps.API.Common.Authorization;

// a hub method checked by AuthorizationHubFilter before it runs:
// [RequirePermission(typeof(GroupsPermissions), nameof(GroupsPermissions.SendMessage), ScopeIdArgument = "groupId")]
// the permission is a static field of the class <Module>Permissions, the argument holds the id of the resource; a
// method may carry several marks, each is checked
[AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
public sealed class RequirePermissionAttribute : Attribute
{
    public RequirePermissionAttribute(Type permissionsType, string permissionName)
    {
        Permission = permissionsType.GetField(permissionName, BindingFlags.Public | BindingFlags.Static)?.GetValue(null) as PermissionDefinition
            ?? throw new InvalidOperationException($"{permissionsType.Name}.{permissionName} is no static PermissionDefinition");
    }

    public PermissionDefinition Permission { get; }

    public string ScopeIdArgument { get; init; } = "";
}
