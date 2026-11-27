using Dara.Server.BuildingBlocks.Application.Authorization;
using Dara.Server.BuildingBlocks.Domain.Exceptions;
using Dara.Server.Modules.Authorization.Application.Roles.GetRoles;
using Dara.Server.Modules.Authorization.Application.Users;
using Dara.Server.Modules.Authorization.Application.Users.AddAccountRole;
using Dara.Server.Modules.Authorization.Application.Users.AddClientRole;
using Dara.Server.Modules.Authorization.Application.Users.CheckPermissions;
using Dara.Server.Modules.Authorization.Application.Users.GetPermissions;
using Dara.Server.Modules.Authorization.Application.Users.RemoveAccountRole;
using Dara.Server.Modules.Authorization.Domain.Permissions;
using Dara.Server.Modules.Authorization.Domain.Roles;
using Dara.Server.Modules.Authorization.Domain.Users;
using Dara.Server.Modules.Authorization.Domain.Users.Clients;
using Dara.Server.Modules.Authorization.Domain.Users.Rules;
using Dara.Server.Modules.Identity.Integration;

namespace Dara.Server.Modules.Authorization.IntegrationTests;

// the catalog of the modules, the roles of accounts and clients from the events of Identity, and the checks of the host
public class AuthorizationTests
{
    private static CancellationToken Token => TestContext.Current.CancellationToken;

    private static async Task<Guid> CreateUserAsync(AuthorizationModuleHost host)
    {
        var userId = Guid.NewGuid();
        await host.EventBus.PublishAsync(new NewUserCreatedIntegrationEvent(Guid.NewGuid(), DateTime.UtcNow, userId));
        await host.WaitUntilAsync(db => db.Query<UserRoles>().Any(u => u.UserId == new UserId(userId)), "roles of the new account");
        return userId;
    }

    private static async Task<Guid> RegisterClientAsync(AuthorizationModuleHost host, Guid userId)
    {
        var clientId = Guid.NewGuid();
        await host.EventBus.PublishAsync(new UserClientRegisteredIntegrationEvent(Guid.NewGuid(), DateTime.UtcNow, clientId, userId, "Pc 1", "Pc"));
        await host.WaitUntilAsync(db => HasClient(db.Query<UserRoles>().ToList(), userId, clientId), "roles of the new client");
        return clientId;
    }

    private static bool HasClient(List<UserRoles> all, Guid userId, Guid clientId) =>
        all.Any(u => u.UserId.Value == userId && u.Clients.Any(c => c.ClientId.Value == clientId));

    private static Task<CallerPermissionsDto> GetPermissionsAsync(AuthorizationModuleHost host, Guid userId, Guid? clientId = null) =>
        host.QueryAsync<GetPermissionsQuery, CallerPermissionsDto>(new GetPermissionsQuery(userId, clientId));

    [Fact]
    public async Task EveryPermissionAndRoleOfTheModulesIsKeptOnceAfterASecondStart()
    {
        await using var host = await AuthorizationModuleHost.StartAsync(Token);

        await host.SynchronizeAsync(AuthorizationModuleHost.TestCatalog);

        var expectedPermissions = host.Root.AuthorizationCatalog.PermissionCodes.Concat(AuthorizationModuleHost.TestCatalog.PermissionCodes).Order().ToList();
        Assert.Equal(expectedPermissions, await host.ReadAsync(db => db.Query<Permission>().Select(p => p.Code).ToList().Order().ToList()));
        var roles = await host.QueryAsync<GetRolesQuery, List<RoleDto>>(new GetRolesQuery());
        Assert.Equal(["AuthorizationRole.Owner", "AuthorizationRole.Viewer", "TestRole.Owner", "TestRole.Viewer"], roles.Select(r => r.Code));
        var testOwner = roles.Single(r => r.Code == "TestRole.Owner");
        Assert.Equal(("Test", false), (testOwner.Module, testOwner.IsDefault));
        Assert.Equal(["TestPermission.Edit", "TestPermission.Read"], testOwner.PermissionCodes);
    }

    [Fact]
    public async Task ANewAccountAndANewClientGetTheDefaultRolesAndARevokedClientLosesThem()
    {
        await using var host = await AuthorizationModuleHost.StartAsync(Token);
        var userId = await CreateUserAsync(host);

        var clientId = await RegisterClientAsync(host, userId);

        var defaults = new[] { "AuthorizationRole.Owner", "AuthorizationRole.Viewer", "TestRole.Viewer" };
        Assert.Equal(defaults, (await GetPermissionsAsync(host, userId)).RoleCodes);
        Assert.Equal(defaults, (await GetPermissionsAsync(host, userId, clientId)).RoleCodes);

        await host.EventBus.PublishAsync(new UserClientRevokedIntegrationEvent(Guid.NewGuid(), DateTime.UtcNow, clientId, userId));
        await host.WaitUntilAsync(db => !HasClient(db.Query<UserRoles>().ToList(), userId, clientId), "the revoked client is gone");
    }

    [Fact]
    public async Task AnAccountRoleCountsForTheAccountOnlyAndTheCheckSaysWhichCodesAreAllowed()
    {
        await using var host = await AuthorizationModuleHost.StartAsync(Token);
        var userId = await CreateUserAsync(host);
        var clientId = await RegisterClientAsync(host, userId);
        var codes = new List<string> { "TestPermission.Edit", "TestPermission.Read", "Unknown.Code" };

        await host.ExecuteAsync(new AddAccountRoleCommand(userId, "TestRole.Owner"));

        var account = await host.QueryAsync<CheckPermissionsQuery, List<PermissionCheckDto>>(new CheckPermissionsQuery(userId, null, codes));
        Assert.Equal([true, true, false], account.Select(c => c.IsAllowed));
        var client = await host.QueryAsync<CheckPermissionsQuery, List<PermissionCheckDto>>(new CheckPermissionsQuery(userId, clientId, codes));
        Assert.Equal([false, true, false], client.Select(c => c.IsAllowed));

        await host.ExecuteAsync(new RemoveAccountRoleCommand(userId, "TestRole.Owner"));
        Assert.DoesNotContain("TestPermission.Edit", (await GetPermissionsAsync(host, userId)).PermissionCodes);
    }

    [Fact]
    public async Task AnUnknownRoleOrAClientOfAnotherAccountIsRefused()
    {
        await using var host = await AuthorizationModuleHost.StartAsync(Token);
        var userId = await CreateUserAsync(host);
        var otherUserId = await CreateUserAsync(host);
        var otherClientId = await RegisterClientAsync(host, otherUserId);

        var unknownRole = await Assert.ThrowsAsync<BuisnessRuleValidationException>(() => host.ExecuteAsync(new AddAccountRoleCommand(userId, "TestRole.Admin")));
        Assert.IsType<RoleMustExistRule>(unknownRole.BrokenRule);
        var foreignClient = await Assert.ThrowsAsync<BuisnessRuleValidationException>(() => host.ExecuteAsync(new AddClientRoleCommand(userId, otherClientId, "TestRole.Owner")));
        Assert.IsType<ClientMustBelongToUserRule>(foreignClient.BrokenRule);
    }

    [Fact]
    public async Task AnAccountOrClientNotKnownYetGetsTheDefaultRoles()
    {
        await using var host = await AuthorizationModuleHost.StartAsync(Token);

        var permissions = await GetPermissionsAsync(host, Guid.NewGuid(), Guid.NewGuid());

        Assert.Equal(["AuthorizationRole.Owner", "AuthorizationRole.Viewer", "TestRole.Viewer"], permissions.RoleCodes);
        Assert.Contains("TestPermission.Read", permissions.PermissionCodes);
        Assert.DoesNotContain("TestPermission.Edit", permissions.PermissionCodes);
    }

    [Fact]
    public async Task ARoleNoModuleDeclaresAnyMoreDisappearsFromTheAccounts()
    {
        await using var host = await AuthorizationModuleHost.StartAsync(Token);
        var userId = await CreateUserAsync(host);
        await host.ExecuteAsync(new AddAccountRoleCommand(userId, "TestRole.Owner"));

        var withoutOwner = AuthorizationModuleHost.TestCatalog with
        {
            Roles = [new ModuleRoleDefinition("TestRole.Viewer", true, ["TestPermission.Edit", "TestPermission.Read"])]
        };
        await host.SynchronizeAsync(withoutOwner);

        Assert.False(await host.ReadAsync(db => db.Query<Role>().Any(r => r.Code == "TestRole.Owner")));
        var permissions = await GetPermissionsAsync(host, userId);
        Assert.DoesNotContain("TestRole.Owner", permissions.RoleCodes);
        Assert.Contains("TestPermission.Edit", permissions.PermissionCodes);
    }
}
