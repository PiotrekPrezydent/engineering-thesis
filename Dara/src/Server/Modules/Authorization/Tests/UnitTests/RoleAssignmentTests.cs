using Dara.Server.BuildingBlocks.Domain.Exceptions;
using Dara.Server.Modules.Authorization.Domain.RoleAssignments;
using Dara.Server.Modules.Authorization.Domain.RoleAssignments.Rules;
using Dara.Server.Modules.Authorization.Domain.Roles;

namespace Dara.Server.Modules.Authorization.UnitTests;

public class RoleAssignmentTests
{
    private const string ManageAccess = "test.item.manage_access";

    private static readonly Role Editor = Role.Register("Test.Editor", "Test.Item", "Test", ["test.item.edit", "test.item.read"]);
    private static readonly Role Owner = Role.Register("Test.Owner", "Test.Item", "Test", ["test.item.edit", ManageAccess, "test.item.read"],
        ["Test.Owner", "Test.Editor"]);
    private static readonly Guid Item = Guid.NewGuid();
    private static readonly Guid Subject = Guid.NewGuid();

    // a manager: manages access and the editor, not the owner
    private static readonly GranterAccess Manager = Granter(["Test.Editor"], ManageAccess, "test.item.read");
    private static readonly GranterAccess ServerAdmin = new(new HashSet<string>(), new HashSet<string>(), ManageAccess, IsServerAdmin: true);

    private static GranterAccess Granter(string[] manageableRoleCodes, params string[] permissionCodes) =>
        new(permissionCodes.ToHashSet(), manageableRoleCodes.ToHashSet(), ManageAccess, IsServerAdmin: false);

    [Fact]
    public void AnAssignmentTakesTheScopeOfItsRoleAndAModuleMayAssignItGlobally()
    {
        var assignment = RoleAssignment.Assign(Subject, Editor, Item);
        var global = RoleAssignment.Assign(Subject, Editor, RoleAssignment.AllResources);

        Assert.Equal(("Test.Editor", "Test.Item", Item, Subject), (assignment.RoleCode, assignment.ScopeCode, assignment.ScopeId, assignment.SubjectId));
        Assert.Equal(Guid.Empty, global.ScopeId);
    }

    [Fact]
    public void AGranterWithoutTheAccessPermissionIsRefused()
    {
        var error = Assert.Throws<BuisnessRuleValidationException>(() =>
            RoleAssignment.Grant(Subject, Editor, Item, Granter(["Test.Editor"], "test.item.read"), new Checker(false)));
        Assert.IsType<GranterMustManageAccessRule>(error.BrokenRule);
    }

    [Fact]
    public void AGranterWithoutARoleThatManagesTheRoleIsRefused()
    {
        var error = Assert.Throws<BuisnessRuleValidationException>(() =>
            RoleAssignment.Grant(Subject, Owner, Item, Manager, new Checker(false)));
        Assert.IsType<GranterMustManageTheRoleRule>(error.BrokenRule);

        RoleAssignment.Grant(Subject, Editor, Item, Manager, new Checker(false));
    }

    [Fact]
    public void TheServerAdminGrantsAnyRoleButNoneGlobally()
    {
        RoleAssignment.Grant(Subject, Owner, Item, ServerAdmin, new Checker(false));

        var error = Assert.Throws<BuisnessRuleValidationException>(() =>
            RoleAssignment.Grant(Subject, Owner, RoleAssignment.AllResources, ServerAdmin, new Checker(false)));
        Assert.IsType<GlobalRoleIsAssignedOnlyAutomaticallyRule>(error.BrokenRule);
    }

    [Fact]
    public void ARoleIsGrantedOncePerSubjectAndResource()
    {
        var error = Assert.Throws<BuisnessRuleValidationException>(() =>
            RoleAssignment.Grant(Subject, Editor, Item, Manager, new Checker(true)));
        Assert.IsType<RoleCannotBeAssignedTwiceRule>(error.BrokenRule);
    }

    [Fact]
    public void ARevokeFollowsTheRulesOfAGrant()
    {
        var owner = RoleAssignment.Assign(Subject, Owner, Item);
        var error = Assert.Throws<BuisnessRuleValidationException>(() => owner.Revoke(Manager, Owner));
        Assert.IsType<GranterMustManageTheRoleRule>(error.BrokenRule);

        RoleAssignment.Assign(Subject, Editor, Item).Revoke(Manager, Editor);

        var global = RoleAssignment.Assign(Subject, Editor, RoleAssignment.AllResources);
        var globalError = Assert.Throws<BuisnessRuleValidationException>(() => global.Revoke(ServerAdmin, Editor));
        Assert.IsType<GlobalRoleIsAssignedOnlyAutomaticallyRule>(globalError.BrokenRule);
    }

    [Fact]
    public void ARoleSynchronizesOnlyWhenItsDefinitionChanged()
    {
        var role = Role.Register("Test.Viewer", "Test.Item", "Test", ["test.item.read"]);

        Assert.False(role.Synchronize("Test.Item", "Test", ["test.item.read", "test.item.read"]));
        Assert.True(role.Synchronize("Test.Item", "Test", ["test.item.read", "test.item.edit"]));
        Assert.Equal(["test.item.edit", "test.item.read"], role.PermissionCodes);
        Assert.True(role.Synchronize("Test.Item", "Test", ["test.item.read", "test.item.edit"], ["Test.Viewer"]));
        Assert.Equal(["Test.Viewer"], role.ManageableRoleCodes);
    }

    private sealed class Checker(bool assigned) : IRoleAssignmentChecker
    {
        public bool IsAssigned(Guid subjectId, string roleCode, Guid scopeId) => assigned;
    }
}
