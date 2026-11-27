using Dara.Server.BuildingBlocks.Application.Authorization;
using Dara.Server.BuildingBlocks.Domain.Exceptions;
using Dara.Server.BuildingBlocks.Integration.Authorization;
using Dara.Server.Modules.Authorization.Application.RoleAssignments;
using Dara.Server.Modules.Authorization.Application.RoleAssignments.AssignServerAdmin;
using Dara.Server.Modules.Authorization.Application.RoleAssignments.GetMyAssignments;
using Dara.Server.Modules.Authorization.Application.RoleAssignments.GrantRole;
using Dara.Server.Modules.Authorization.Application.RoleAssignments.IsAuthorized;
using Dara.Server.Modules.Authorization.Application.RoleAssignments.RevokeRole;
using Dara.Server.Modules.Authorization.Domain.Permissions;
using Dara.Server.Modules.Authorization.Domain.RoleAssignments;
using Dara.Server.Modules.Authorization.Domain.RoleAssignments.Rules;
using Dara.Server.Modules.Authorization.Domain.Roles;

namespace Dara.Server.Modules.Authorization.IntegrationTests;

public class AuthorizationTests
{
    private static CancellationToken Token => TestContext.Current.CancellationToken;

    [Fact]
    public async Task TheStartSynchronizesTheDefinitionsOnceWithFlattenedRoles()
    {
        await using var host = await AuthorizationModuleHost.StartAsync(Token);
        await host.SynchronizeAsync(TestDefinition.Instance);

        var permissions = await host.ReadAsync(r => r.Query<Permission>().Where(p => p.Module == "Test").ToList());
        var roles = await host.ReadAsync(r => r.Query<Role>().Where(x => x.Module == "Test").ToList());

        Assert.Equal(["test.item.edit", "test.item.manage_access", "test.item.part.manage_access", "test.item.part.use", "test.item.read"],
            permissions.Select(p => p.Code).Order(StringComparer.Ordinal));
        Assert.Equal(["Test.Editor", "Test.Manager", "Test.Owner", "Test.PartOwner", "Test.Viewer"], roles.Select(r => r.Code).Order(StringComparer.Ordinal));
        Assert.All(roles.Where(r => r.Code != "Test.PartOwner"), r => Assert.Equal("Test.Item", r.ScopeCode));
        // the subordinate of the child scope is a companion, its permissions are not flattened into the owner
        var owner = roles.Single(r => r.Code == "Test.Owner");
        Assert.Equal(["test.item.edit", "test.item.manage_access", "test.item.read"], owner.PermissionCodes);
        Assert.Equal(["Test.PartOwner"], owner.CompanionRoleCodes);
        Assert.Equal(["Test.Editor", "Test.Manager", "Test.Viewer"], roles.Single(r => r.Code == "Test.Manager").ManageableRoleCodes);
        Assert.True(await host.ReadAsync(r => r.Query<Role>().Any(x => x.Code == "Authorization.ServerAdmin" && x.ScopeCode == "Authorization.Server")));
    }

    [Fact]
    public async Task ACodeTwoModulesDeclareStopsTheStart()
    {
        await using var host = await AuthorizationModuleHost.StartAsync(Token);

        var error = await Assert.ThrowsAsync<InvalidOperationException>(() => host.SynchronizeAsync(TestDefinition.Instance, new CollidingDefinition()));
        Assert.Contains("permission test.item.read is declared by Test and Other", error.Message);
    }

    [Fact]
    public async Task ARoleRemovedFromTheCodeDisappearsWithItsAssignments()
    {
        await using var host = await AuthorizationModuleHost.StartAsync(Token);
        var subject = Guid.NewGuid();
        var item = Guid.NewGuid();
        await GrantByEventAsync(host, subject, TestDefinition.Editor, item);
        await GrantByEventAsync(host, subject, TestDefinition.Viewer, item);

        await host.SynchronizeAsync(TestDefinition.Instance with { Roles = [TestDefinition.Owner, TestDefinition.Manager, TestDefinition.Viewer] });

        Assert.False(await host.ReadAsync(r => r.Query<Role>().Any(x => x.Code == "Test.Editor")));
        var left = await host.ReadAsync(r => r.Query<RoleAssignment>().Where(a => a.SubjectId == subject).Select(a => a.RoleCode).ToList());
        Assert.Equal(["Test.Viewer"], left);
    }

    [Fact]
    public async Task AnAssignmentAllowsOnlyItsSubjectOnThatResource()
    {
        await using var host = await AuthorizationModuleHost.StartAsync(Token);
        var client = Guid.NewGuid();
        var item = Guid.NewGuid();
        await GrantByEventAsync(host, client, TestDefinition.Editor, item);

        Assert.True(await IsAuthorizedAsync(host, client, "test.item.edit", item));
        // read comes from the subordinate role
        Assert.True(await IsAuthorizedAsync(host, client, "test.item.read", item));
        Assert.False(await IsAuthorizedAsync(host, client, "test.item.manage_access", item));
        Assert.False(await IsAuthorizedAsync(host, client, "test.item.edit", Guid.NewGuid()));
        // nobody else, not even the user of the client, gets it
        Assert.False(await IsAuthorizedAsync(host, Guid.NewGuid(), "test.item.read", item));

        var mine = await host.QueryAsync<GetMyAssignmentsQuery, List<RoleAssignmentDto>>(new GetMyAssignmentsQuery(client));
        var assignment = Assert.Single(mine);
        Assert.Equal(("Test.Editor", item), (assignment.RoleCode, assignment.ScopeId));
        Assert.Equal(["test.item.edit", "test.item.read"], assignment.PermissionCodes);
    }

    [Fact]
    public async Task AGlobalAssignmentAllowsOnEveryResourceOfTheScope()
    {
        await using var host = await AuthorizationModuleHost.StartAsync(Token);
        var subject = Guid.NewGuid();
        await GrantByEventAsync(host, subject, TestDefinition.Viewer, ScopeIds.All);

        Assert.True(await IsAuthorizedAsync(host, subject, "test.item.read", Guid.NewGuid()));
        Assert.True(await IsAuthorizedAsync(host, subject, "test.item.read", Guid.NewGuid()));
        Assert.False(await IsAuthorizedAsync(host, subject, "test.item.edit", Guid.NewGuid()));
    }

    [Fact]
    public async Task ARevokeEventAndARemovedScopeTakeTheAccessAway()
    {
        await using var host = await AuthorizationModuleHost.StartAsync(Token);
        var user = Guid.NewGuid();
        var client = Guid.NewGuid();
        var item = Guid.NewGuid();
        await GrantByEventAsync(host, user, TestDefinition.Viewer, item);
        await GrantByEventAsync(host, user, TestDefinition.Owner, client);
        await GrantByEventAsync(host, client, TestDefinition.Owner, item);
        await GrantByEventAsync(host, client, TestDefinition.Viewer, Guid.NewGuid());

        await host.EventBus.PublishAsync(new RevokeScopeRoleIntegrationEvent(Guid.NewGuid(), DateTime.UtcNow, user,
            TestDefinition.Viewer.Code, TestDefinition.Item.Code, item));
        await host.WaitUntilAsync(r => !r.Query<RoleAssignment>().Any(a => a.SubjectId == user && a.ScopeId == item), "the revoke event removed the assignment");

        // the client is removed as a resource: the assignments on it and its assignments as a subject go
        await host.EventBus.PublishAsync(new ScopeRemovedIntegrationEvent(Guid.NewGuid(), DateTime.UtcNow, "Test.Item", client));
        await host.WaitUntilAsync(r => !r.Query<RoleAssignment>().Any(a => a.SubjectId == client || a.ScopeId == client),
            "the removed scope took the assignments on and of the client");
    }

    [Fact]
    public async Task AGranterGrantsAndRevokesOnlyTheRolesItsRolesManage()
    {
        await using var host = await AuthorizationModuleHost.StartAsync(Token);
        var owner = Guid.NewGuid();
        var manager = Guid.NewGuid();
        var editor = Guid.NewGuid();
        var other = Guid.NewGuid();
        var item = Guid.NewGuid();
        await GrantByEventAsync(host, owner, TestDefinition.Owner, item);
        await GrantByEventAsync(host, manager, TestDefinition.Manager, item);
        await GrantByEventAsync(host, editor, TestDefinition.Editor, item);

        await AssertBrokenAsync<GranterMustManageAccessRule>(host, new GrantRoleCommand(editor, other, TestDefinition.Viewer.Code, item));
        await AssertBrokenAsync<GranterMustManageTheRoleRule>(host, new GrantRoleCommand(manager, other, TestDefinition.Owner.Code, item));
        // on another item the manager manages nothing
        await AssertBrokenAsync<GranterMustManageAccessRule>(host, new GrantRoleCommand(manager, other, TestDefinition.Editor.Code, Guid.NewGuid()));

        await host.ExecuteAsync(new GrantRoleCommand(manager, other, TestDefinition.Editor.Code, item));
        Assert.True(await IsAuthorizedAsync(host, other, "test.item.edit", item));
        await AssertBrokenAsync<RoleCannotBeAssignedTwiceRule>(host, new GrantRoleCommand(manager, other, TestDefinition.Editor.Code, item));

        await AssertBrokenAsync<GranterMustManageTheRoleRule>(host, new RevokeRoleCommand(manager, owner, TestDefinition.Owner.Code, item));
        await host.ExecuteAsync(new RevokeRoleCommand(owner, manager, TestDefinition.Manager.Code, item));
        Assert.False(await IsAuthorizedAsync(host, manager, "test.item.manage_access", item));
        await AssertBrokenAsync<RoleMustBeAssignedRule>(host, new RevokeRoleCommand(owner, manager, TestDefinition.Manager.Code, item));

        await AssertBrokenAsync<GlobalRoleIsAssignedOnlyAutomaticallyRule>(host, new GrantRoleCommand(owner, other, TestDefinition.Viewer.Code, ScopeIds.All));
    }

    [Fact]
    public async Task AGrantPassesTheCompanionRolesOnResourcesTheGranterManagesAllOrNothing()
    {
        await using var host = await AuthorizationModuleHost.StartAsync(Token);
        var owner = Guid.NewGuid();
        var heir = Guid.NewGuid();
        var other = Guid.NewGuid();
        var item = Guid.NewGuid();
        var part = Guid.NewGuid();
        var foreignPart = Guid.NewGuid();
        await GrantByEventAsync(host, owner, TestDefinition.Owner, item);
        await GrantByEventAsync(host, owner, TestDefinition.PartOwner, part);
        var withPart = new List<CompanionAssignment> { new(TestDefinition.PartOwner.Code, part) };

        // a part the owner holds nothing on: the whole grant fails, the item role is not given either
        await AssertBrokenAsync<GranterMustManageAccessRule>(host, new GrantRoleCommand(owner, other, TestDefinition.Owner.Code, item,
            [.. withPart, new CompanionAssignment(TestDefinition.PartOwner.Code, foreignPart)]));
        Assert.False(await host.ReadAsync(r => r.Query<RoleAssignment>().Any(a => a.SubjectId == other)));
        await AssertBrokenAsync<CompanionMustBelongToTheRoleRule>(host, new GrantRoleCommand(owner, other, TestDefinition.Owner.Code, item,
            [new CompanionAssignment(TestDefinition.Viewer.Code, part)]));

        await host.ExecuteAsync(new GrantRoleCommand(owner, heir, TestDefinition.Owner.Code, item, withPart));
        Assert.True(await IsAuthorizedAsync(host, heir, "test.item.manage_access", item));
        Assert.True(await IsAuthorizedAsync(host, heir, "test.item.part.use", part));
        Assert.False(await IsAuthorizedAsync(host, heir, "test.item.part.use", item));

        await host.ExecuteAsync(new RevokeRoleCommand(owner, heir, TestDefinition.Owner.Code, item, withPart));
        Assert.False(await host.ReadAsync(r => r.Query<RoleAssignment>().Any(a => a.SubjectId == heir)));
    }

    [Fact]
    public async Task TheServerAdminGrantsAndRevokesEveryRoleOnEveryResource()
    {
        await using var host = await AuthorizationModuleHost.StartAsync(Token);
        var admin = Guid.NewGuid();
        var owner = Guid.NewGuid();
        var item = Guid.NewGuid();
        await host.ExecuteAsync(new AssignServerAdminCommand(admin));
        await host.ExecuteAsync(new AssignServerAdminCommand(admin));
        Assert.Equal(1, await host.ReadAsync(r => r.Query<RoleAssignment>().Count(a => a.SubjectId == admin)));

        await host.ExecuteAsync(new GrantRoleCommand(admin, owner, TestDefinition.Owner.Code, item));
        Assert.True(await IsAuthorizedAsync(host, owner, "test.item.manage_access", item));
        await host.ExecuteAsync(new RevokeRoleCommand(admin, owner, TestDefinition.Owner.Code, item));
        Assert.False(await IsAuthorizedAsync(host, owner, "test.item.read", item));

        // the admin grants roles, it does not hold their permissions
        Assert.False(await IsAuthorizedAsync(host, admin, "test.item.read", item));
        await AssertBrokenAsync<GlobalRoleIsAssignedOnlyAutomaticallyRule>(host, new GrantRoleCommand(admin, owner, TestDefinition.Owner.Code, ScopeIds.All));
    }

    private static async Task AssertBrokenAsync<TRule>(AuthorizationModuleHost host, BuildingBlocks.Application.Commands.ICommand command)
    {
        var error = await Assert.ThrowsAsync<BuisnessRuleValidationException>(() => command switch
        {
            GrantRoleCommand grant => host.ExecuteAsync(grant),
            RevokeRoleCommand revoke => host.ExecuteAsync(revoke),
            _ => throw new ArgumentOutOfRangeException(nameof(command))
        });
        Assert.IsType<TRule>(error.BrokenRule);
    }

    private static async Task GrantByEventAsync(AuthorizationModuleHost host, Guid subjectId, RoleDefinition role, Guid scopeId)
    {
        await host.EventBus.PublishAsync(new GrantScopeRoleIntegrationEvent(Guid.NewGuid(), DateTime.UtcNow, subjectId,
            role.Code, role.Scope.Code, scopeId));
        await host.WaitUntilAsync(r => r.Query<RoleAssignment>().Any(a => a.SubjectId == subjectId && a.RoleCode == role.Code && a.ScopeId == scopeId),
            $"{role.Code} assigned by the event");
    }

    private static Task<bool> IsAuthorizedAsync(AuthorizationModuleHost host, Guid senderId, string permissionCode, Guid scopeId)
    {
        return host.QueryAsync<IsAuthorizedQuery, bool>(new IsAuthorizedQuery(senderId, permissionCode, scopeId));
    }

    private sealed class CollidingDefinition : IModuleAuthorizationDefinition
    {
        private static readonly ScopeDefinition Scope = new("Other.Item");
        private static readonly PermissionDefinition Read = new("test.item.read", Scope);

        public string ModuleName => "Other";
        public IEnumerable<ScopeDefinition> GetScopes() => [Scope];
        public IEnumerable<PermissionDefinition> GetPermissions() => [Read];
        public IEnumerable<RoleDefinition> GetDefaultRoles() => [new RoleDefinition("Other.Viewer", Scope, [Read])];
    }
}
