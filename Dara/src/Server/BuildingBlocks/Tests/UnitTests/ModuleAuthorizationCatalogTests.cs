using Dara.Server.BuildingBlocks.Application.Authorization;
using Dara.Server.BuildingBlocks.Domain.Roles;

namespace Dara.Server.BuildingBlocks.UnitTests;

// the start-up check of ModuleCompositionRootBase.Initialize on the types of a test module "Test"
public class ModuleAuthorizationCatalogTests
{
    [Fact]
    public void CodesAreTheTypeNameAndTheValue()
    {
        Assert.Equal("TestPermission.Edit", new TestPermission(TestPermission.Permissions.Edit).PermissionCode);
        Assert.Equal("TestRole.Owner", new TestRole(TestRole.Roles.Owner).RoleCode);
    }

    [Fact]
    public void AValidModuleGivesItsPermissionsAndRoles()
    {
        var catalog = ModuleAuthorizationCatalog.FromTypes("Test", [typeof(TestPermission), typeof(ValidRoles)]);

        Assert.Equal("Test", catalog.Module);
        Assert.Equal(["TestPermission.Edit", "TestPermission.Read", "TestPermission.Remove"], catalog.PermissionCodes);
        Assert.Collection(catalog.Roles,
            owner =>
            {
                Assert.Equal("TestRole.Owner", owner.RoleCode);
                Assert.True(owner.IsDefault);
                Assert.Equal(catalog.PermissionCodes, owner.PermissionCodes);
            },
            viewer =>
            {
                Assert.Equal("TestRole.Viewer", viewer.RoleCode);
                Assert.False(viewer.IsDefault);
                Assert.Equal(["TestPermission.Read"], viewer.PermissionCodes);
            });
    }

    [Fact]
    public void APermissionInNoRoleStopsTheModuleAndTheErrorNamesTheModuleAndTheCode()
    {
        var error = Assert.Throws<InvalidOperationException>(() =>
            ModuleAuthorizationCatalog.FromTypes("Test", [typeof(TestPermission), typeof(UncoveredRoles)]));

        Assert.Contains("module Test", error.Message);
        Assert.EndsWith("permissions that no role allows: TestPermission.Remove", error.Message);
    }

    [Fact]
    public void AModuleWithoutRolesDoesNotStart()
    {
        var error = Assert.Throws<InvalidOperationException>(() =>
            ModuleAuthorizationCatalog.FromTypes("Test", [typeof(TestPermission)]));

        Assert.Contains("module Test", error.Message);
        Assert.Contains(nameof(IModuleRoles), error.Message);
    }

    [Fact]
    public void AModuleWithTwoRoleClassesDoesNotStart()
    {
        var error = Assert.Throws<InvalidOperationException>(() =>
            ModuleAuthorizationCatalog.FromTypes("Test", [typeof(TestPermission), typeof(ValidRoles), typeof(UncoveredRoles)]));

        Assert.Contains("found 2", error.Message);
    }

    [Fact]
    public void ARoleCannotAllowAPermissionOfAnotherModule()
    {
        var error = Assert.Throws<InvalidOperationException>(() =>
            ModuleAuthorizationCatalog.FromTypes("Test", [typeof(TestPermission), typeof(ForeignRoles)]));

        Assert.Contains("OtherPermission.Send", error.Message);
    }

    [Fact]
    public void RoleCodesAreUniqueAndNoRoleIsEmpty()
    {
        var duplicate = Assert.Throws<InvalidOperationException>(() =>
            ModuleAuthorizationCatalog.FromTypes("Test", [typeof(TestPermission), typeof(DuplicateRoles)]));
        var empty = Assert.Throws<InvalidOperationException>(() =>
            ModuleAuthorizationCatalog.FromTypes("Test", [typeof(TestPermission), typeof(EmptyRoles)]));

        Assert.Contains("TestRole.Owner is defined more than once", duplicate.Message);
        Assert.Contains("TestRole.Viewer allows no permission", empty.Message);
    }

    public sealed record TestPermission(TestPermission.Permissions Value) : ModulePermission<TestPermission.Permissions>(Value)
    {
        public enum Permissions { Read, Edit, Remove }
    }

    public sealed record OtherPermission(OtherPermission.Permissions Value) : ModulePermission<OtherPermission.Permissions>(Value)
    {
        public enum Permissions { Send }
    }

    public sealed record TestRole(TestRole.Roles Value) : EntityRole<TestRole.Roles>(Value)
    {
        public enum Roles { Owner, Viewer }
    }

    internal sealed class ValidRoles : IModuleRoles
    {
        public void Configure(ModuleRolesBuilder roles) => roles
            .Role(new TestRole(TestRole.Roles.Owner), isDefault: true)
                .AllowsAll<TestPermission>()
            .Role(new TestRole(TestRole.Roles.Viewer))
                .Allows(new TestPermission(TestPermission.Permissions.Read));
    }

    internal sealed class UncoveredRoles : IModuleRoles
    {
        public void Configure(ModuleRolesBuilder roles) => roles
            .Role(new TestRole(TestRole.Roles.Owner))
                .Allows(new TestPermission(TestPermission.Permissions.Read), new TestPermission(TestPermission.Permissions.Edit));
    }

    internal sealed class ForeignRoles : IModuleRoles
    {
        public void Configure(ModuleRolesBuilder roles) => roles
            .Role(new TestRole(TestRole.Roles.Owner))
                .AllowsAll<TestPermission>()
                .Allows(new OtherPermission(OtherPermission.Permissions.Send));
    }

    internal sealed class DuplicateRoles : IModuleRoles
    {
        public void Configure(ModuleRolesBuilder roles) => roles
            .Role(new TestRole(TestRole.Roles.Owner)).AllowsAll<TestPermission>()
            .Role(new TestRole(TestRole.Roles.Owner)).AllowsAll<TestPermission>();
    }

    internal sealed class EmptyRoles : IModuleRoles
    {
        public void Configure(ModuleRolesBuilder roles) => roles
            .Role(new TestRole(TestRole.Roles.Owner)).AllowsAll<TestPermission>()
            .Role(new TestRole(TestRole.Roles.Viewer));
    }
}
