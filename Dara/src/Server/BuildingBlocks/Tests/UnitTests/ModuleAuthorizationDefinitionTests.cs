using Dara.Server.BuildingBlocks.Application.Authorization;
using Dara.Server.BuildingBlocks.Integration.Authorization;

namespace Dara.Server.BuildingBlocks.UnitTests;

// the start-up check of ModuleCompositionRootBase.Initialize on the definition of a test module "Test"
public class ModuleAuthorizationDefinitionTests
{
    private static readonly ScopeDefinition Item = new("Test.Item");
    private static readonly ScopeDefinition Other = new("Test.Other");
    private static readonly PermissionDefinition Read = new("test.item.read", Item);
    private static readonly PermissionDefinition Edit = new("test.item.edit", Item);
    private static readonly PermissionDefinition ManageAccess = new(Item.ManageAccessPermissionCode, Item);
    private static readonly PermissionDefinition OtherRead = new("test.other.read", Other);

    private static readonly RoleDefinition Viewer = new("Test.Viewer", Item, [Read]);
    private static readonly RoleDefinition Editor = new("Test.Editor", Item, [Edit]) { Subordinates = [Viewer] };
    private static readonly RoleDefinition Owner = new("Test.Owner", Item, [ManageAccess])
    {
        Subordinates = [Editor],
        ManageableRoleCodes = ["Test.Owner", "Test.Editor", "Test.Viewer"]
    };
    private static readonly RoleDefinition OtherViewer = new("Test.OtherViewer", Other, [OtherRead]);

    private static TestDefinition Valid() => new([Item, Other], [Read, Edit, ManageAccess, OtherRead], [Owner, Editor, Viewer, OtherViewer])
    {
        NewUserRoles = [new NewSubjectRole(Viewer), new NewSubjectRole(Owner, OnItself: true)]
    };

    [Fact]
    public void AValidDefinitionPassesAndARoleHasThePermissionsOfItsSubordinates()
    {
        ModuleAuthorizationDefinitionValidator.Validate("Test", Valid());

        Assert.Equal(["test.item.edit", "test.item.manage_access", "test.item.read"], Owner.AllPermissions().Select(p => p.Code).Order(StringComparer.Ordinal));
    }

    [Fact]
    public void ThePermissionThatManagesAccessFollowsThePathOfItsScope()
    {
        Assert.Equal("test.item.manage_access", Item.ManageAccessPermissionCode);
        Assert.True(ManageAccess.ManagesAccess);
        Assert.False(Read.ManagesAccess);
        Assert.False(new PermissionDefinition("test.item.function.manage_access", Item).ManagesAccess);
    }

    [Fact]
    public void ANewSubjectRoleIsGlobalOrOnTheSubjectItself()
    {
        var subject = Guid.NewGuid();

        var global = new NewSubjectRole(Viewer).ToGrant(subject, DateTime.UtcNow);
        var onItself = new NewSubjectRole(Owner, OnItself: true).ToGrant(subject, DateTime.UtcNow);

        Assert.Equal((subject, "Test.Viewer", "Test.Item", ScopeIds.All), (global.SubjectId, global.RoleCode, global.ScopeCode, global.ScopeId));
        Assert.Equal(subject, onItself.ScopeId);
    }

    [Fact]
    public void APermissionInNoRoleStopsTheModule()
    {
        var definition = Valid() with { Roles = [Editor, Viewer, OtherViewer], NewUserRoles = [] };

        var error = Assert.Throws<InvalidOperationException>(() => ModuleAuthorizationDefinitionValidator.Validate("Test", definition));
        Assert.Contains("module Test", error.Message);
        Assert.Contains("permissions that no role allows: test.item.manage_access", error.Message);
    }

    [Fact]
    public void ARoleWithAPermissionOfAnotherScopeStopsTheModule()
    {
        var definition = Valid() with { Roles = [.. Valid().Roles, new RoleDefinition("Test.Mixed", Item, [Read, OtherRead])] };

        var error = Assert.Throws<InvalidOperationException>(() => ModuleAuthorizationDefinitionValidator.Validate("Test", definition));
        Assert.Contains("role Test.Mixed of scope Test.Item allows permissions of another scope: test.other.read", error.Message);
    }

    [Fact]
    public void ACodeOutsideThePathStopsTheModule()
    {
        var unscoped = new PermissionDefinition("test.read", Item);
        var definition = Valid() with
        {
            Permissions = [.. Valid().Permissions, unscoped],
            Roles = [.. Valid().Roles, new RoleDefinition("Plugins.Foreign", Item, [unscoped])]
        };

        var error = Assert.Throws<InvalidOperationException>(() => ModuleAuthorizationDefinitionValidator.Validate("Test", definition));
        Assert.Contains("permission test.read is not test.item.<action>", error.Message);
        Assert.Contains("code Plugins.Foreign does not start with Test.", error.Message);
    }

    [Fact]
    public void ASubordinateOfAChildScopeIsACompanionRoleNotFlattened()
    {
        var part = new ScopeDefinition("Test.Item.Part");
        var usePart = new PermissionDefinition("test.item.part.use", part);
        var partOwner = new RoleDefinition("Test.PartOwner", part, [usePart]);
        var owner = Owner with { Subordinates = [Editor, partOwner] };
        var definition = Valid() with
        {
            Scopes = [.. Valid().Scopes, part],
            Permissions = [.. Valid().Permissions, usePart],
            Roles = [owner, Editor, Viewer, OtherViewer, partOwner]
        };

        ModuleAuthorizationDefinitionValidator.Validate("Test", definition);
        Assert.DoesNotContain("test.item.part.use", owner.AllPermissions().Select(p => p.Code));
        Assert.Equal(["Test.PartOwner"], owner.CompanionRoles().Select(r => r.Code));
        // a permission of the child scope declared on the parent breaks the path rule
        var nested = new PermissionDefinition("test.item.part.read", Item);
        var invalid = definition with { Permissions = [.. definition.Permissions, nested], Roles = [.. definition.Roles, new RoleDefinition("Test.Nested", Item, [nested])] };
        var error = Assert.Throws<InvalidOperationException>(() => ModuleAuthorizationDefinitionValidator.Validate("Test", invalid));
        Assert.Contains("permission test.item.part.read is not test.item.<action>", error.Message);
    }

    [Fact]
    public void AnAccessPermissionOfAnotherPathStopsTheModule()
    {
        var functionAccess = new PermissionDefinition("test.item.function.manage_access", Item);
        var definition = Valid() with
        {
            Permissions = [.. Valid().Permissions, functionAccess],
            Roles = [.. Valid().Roles, new RoleDefinition("Test.FunctionManager", Item, [functionAccess])]
        };

        var error = Assert.Throws<InvalidOperationException>(() => ModuleAuthorizationDefinitionValidator.Validate("Test", definition));
        Assert.Contains("permission test.item.function.manage_access would manage access, which in scope Test.Item only test.item.manage_access does", error.Message);
    }

    [Fact]
    public void ADuplicateCodeAnUndeclaredScopeAndAnEmptyRoleStopTheModule()
    {
        var unknown = new ScopeDefinition("Test.Unknown");
        var definition = Valid() with
        {
            Permissions = [.. Valid().Permissions, new PermissionDefinition("test.item.read", Item), new PermissionDefinition("test.unknown.lost", unknown)],
            Roles = [.. Valid().Roles, new RoleDefinition("Test.Viewer", Item, [Read]), new RoleDefinition("Test.Empty", Item, [])]
        };

        var error = Assert.Throws<InvalidOperationException>(() => ModuleAuthorizationDefinitionValidator.Validate("Test", definition));
        Assert.Contains("permission test.item.read is declared more than once", error.Message);
        Assert.Contains("role Test.Viewer is declared more than once", error.Message);
        Assert.Contains("permission test.unknown.lost is in scope Test.Unknown the module does not declare", error.Message);
        Assert.Contains("role Test.Empty allows no permission", error.Message);
    }

    [Fact]
    public void SubordinatesAndManagedRolesStayInTheScopeWithoutCycles()
    {
        var first = new RoleDefinition("Test.First", Item, [Read]);
        var second = new RoleDefinition("Test.Second", Item, [Read]) { Subordinates = [first] };
        var cyclic = first with { Subordinates = [second] };
        var definition = Valid() with
        {
            Roles =
            [
                .. Valid().Roles, cyclic, second,
                new RoleDefinition("Test.Mixed", Item, [Read]) { Subordinates = [OtherViewer] },
                new RoleDefinition("Test.Boss", Item, [ManageAccess]) { ManageableRoleCodes = ["Test.Nobody", "Test.OtherViewer"] }
            ]
        };

        var error = Assert.Throws<InvalidOperationException>(() => ModuleAuthorizationDefinitionValidator.Validate("Test", definition));
        Assert.Contains("role Test.First is its own subordinate", error.Message);
        Assert.Contains("role Test.Mixed of scope Test.Item has subordinate Test.OtherViewer of scope Test.Other", error.Message);
        Assert.Contains("role Test.Boss manages role Test.Nobody the module does not declare", error.Message);
        Assert.Contains("role Test.Boss of scope Test.Item manages role Test.OtherViewer of scope Test.Other", error.Message);
    }

    [Fact]
    public void ARoleThatManagesRolesWithoutManagingAccessStopsTheModule()
    {
        var definition = Valid() with { Roles = [.. Valid().Roles, new RoleDefinition("Test.Sharer", Item, [Read]) { ManageableRoleCodes = ["Test.Viewer"] }] };

        var error = Assert.Throws<InvalidOperationException>(() => ModuleAuthorizationDefinitionValidator.Validate("Test", definition));
        Assert.Contains("role Test.Sharer manages roles without test.item.manage_access", error.Message);
    }

    [Fact]
    public void ANewSubjectRoleTheModuleDoesNotDeclareStopsTheModule()
    {
        var definition = Valid() with { NewUserRoles = [new NewSubjectRole(new RoleDefinition("Test.Ghost", Item, [Read]))] };

        var error = Assert.Throws<InvalidOperationException>(() => ModuleAuthorizationDefinitionValidator.Validate("Test", definition));
        Assert.Contains("new subjects get role Test.Ghost the module does not declare", error.Message);
    }

    [Fact]
    public void AnAssemblyWithoutADefinitionStopsTheModule()
    {
        var error = Assert.Throws<InvalidOperationException>(() =>
            ModuleAuthorizationDefinitionValidator.FromAssembly("Test", typeof(object).Assembly));
        Assert.Contains("module Test must have exactly one implementation of IModuleAuthorizationDefinition, found 0", error.Message);
    }

    private sealed record TestDefinition(
        IReadOnlyList<ScopeDefinition> Scopes,
        IReadOnlyList<PermissionDefinition> Permissions,
        IReadOnlyList<RoleDefinition> Roles) : IModuleAuthorizationDefinition
    {
        public IReadOnlyList<NewSubjectRole> NewUserRoles { get; init; } = [];

        public string ModuleName => "Test";
        public IEnumerable<ScopeDefinition> GetScopes() => Scopes;
        public IEnumerable<PermissionDefinition> GetPermissions() => Permissions;
        public IEnumerable<RoleDefinition> GetDefaultRoles() => Roles;
        public IEnumerable<NewSubjectRole> GetNewUserRoles() => NewUserRoles;
    }
}
