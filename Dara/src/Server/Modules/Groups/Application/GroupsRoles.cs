using Dara.Server.BuildingBlocks.Application.Authorization;

namespace Dara.Server.Modules.Groups.Application;

public static class GroupsRoles
{
    private const string OwnerCode = "Groups.Owner";

    // only reads
    public static readonly RoleDefinition Reader = new("Groups.Reader", GroupsScopes.Group, [GroupsPermissions.Read]);

    // reads and writes
    public static readonly RoleDefinition Member = new("Groups.Member", GroupsScopes.Group, [GroupsPermissions.SendMessage])
    {
        Subordinates = [Reader]
    };

    // the one who created the group: everything a member may, manages the members and the access
    public static readonly RoleDefinition Owner = new(OwnerCode, GroupsScopes.Group, [GroupsPermissions.ManageMembers, GroupsPermissions.ManageAccess])
    {
        Subordinates = [Member],
        ManageableRoleCodes = [OwnerCode, Member.Code, Reader.Code]
    };
}
