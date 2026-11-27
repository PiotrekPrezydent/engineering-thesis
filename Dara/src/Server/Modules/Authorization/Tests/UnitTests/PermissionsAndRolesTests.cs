using Dara.Server.Modules.Authorization.Domain.Permissions;
using Dara.Server.Modules.Authorization.Domain.Roles;

namespace Dara.Server.Modules.Authorization.UnitTests;

// the read-only catalog of the modules: permissions and roles change only through the synchronization
public class PermissionsAndRolesTests
{
    [Fact]
    public void APermissionKeepsItsCodeAndModule()
    {
        var permission = Permission.Register("GroupsPermission.SendMessage", "Groups");

        Assert.Equal("GroupsPermission.SendMessage", permission.Code);
        Assert.Equal("Groups", permission.Module);
    }

    [Fact]
    public void ARoleKeepsItsPermissionCodesOnceAndInOrder()
    {
        var role = Role.Register("GroupsRole.Viewer", "Groups", ["GroupsPermission.ReadGroups", "GroupsPermission.JoinGroup", "GroupsPermission.ReadGroups"], isDefault: true);

        Assert.Equal(["GroupsPermission.JoinGroup", "GroupsPermission.ReadGroups"], role.PermissionCodes);
        Assert.True(role.IsDefault);
    }

    [Fact]
    public void SynchronizationChangesARoleOnlyWhenTheModuleChangedIt()
    {
        var role = Role.Register("GroupsRole.Viewer", "Groups", ["GroupsPermission.ReadGroups"], isDefault: true);

        Assert.False(role.Synchronize(["GroupsPermission.ReadGroups"], isDefault: true));
        Assert.True(role.Synchronize(["GroupsPermission.ReadGroups", "GroupsPermission.JoinGroup"], isDefault: false));

        Assert.Equal(["GroupsPermission.JoinGroup", "GroupsPermission.ReadGroups"], role.PermissionCodes);
        Assert.False(role.IsDefault);
    }
}
