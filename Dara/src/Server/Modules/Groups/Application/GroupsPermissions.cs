using Dara.Server.BuildingBlocks.Application.Authorization;

namespace Dara.Server.Modules.Groups.Application;

// what a subject may do on one resource of a scope of the module, the code starts with the path of the scope; the
// endpoints of the module are marked with them once the owner decides who may do what (TASK-ENG-040-D08)
public static class GroupsPermissions
{
    public static readonly PermissionDefinition Read = new("groups.group.read", GroupsScopes.Group);
    public static readonly PermissionDefinition SendMessage = new("groups.group.send_message", GroupsScopes.Group);
    public static readonly PermissionDefinition ManageMembers = new("groups.group.manage_members", GroupsScopes.Group);
    public static readonly PermissionDefinition ManageAccess = new(GroupsScopes.Group.ManageAccessPermissionCode, GroupsScopes.Group);
}
