using Dara.Server.BuildingBlocks.Domain;
using Dara.Server.BuildingBlocks.Domain.Exceptions;
using Dara.Server.Modules.Authorization.Domain.Roles;
using Dara.Server.Modules.Authorization.Domain.Users;
using Dara.Server.Modules.Authorization.Domain.Users.Clients;
using Dara.Server.Modules.Authorization.Domain.Users.Events;
using Dara.Server.Modules.Authorization.Domain.Users.Rules;

namespace Dara.Server.Modules.Authorization.UnitTests;

// the roles of an account and of its clients: default roles at the start, changes only through UserRoles
public class UserRolesTests
{
    private static readonly string[] Defaults = ["GroupsRole.Owner", "GroupsRole.Viewer"];
    private static readonly FakeRoleExistenceChecker Checker = new("GroupsRole.Owner", "GroupsRole.Viewer", "AuthorizationRole.Owner");

    private static UserRoles NewUserRoles()
    {
        var userRoles = UserRoles.Create(new UserId(Guid.NewGuid()), Defaults);
        userRoles.ClearDomainEvents();
        return userRoles;
    }

    private static void AssertBroken<TRule>(Action action) where TRule : IBuisnessRule
    {
        var refused = Assert.Throws<BuisnessRuleValidationException>(action);
        Assert.IsType<TRule>(refused.BrokenRule);
    }

    [Fact]
    public void ANewAccountAndANewClientGetTheDefaultRoles()
    {
        var userRoles = NewUserRoles();
        var clientId = new UserClientId(Guid.NewGuid());

        userRoles.AddClient(clientId, Defaults);

        Assert.Equal(Defaults, userRoles.AccountRoleCodes);
        Assert.Equal(Defaults, Assert.Single(userRoles.Clients).RoleCodes);
    }

    [Fact]
    public void AClientAddedAgainKeepsItsRoles()
    {
        var userRoles = NewUserRoles();
        var clientId = new UserClientId(Guid.NewGuid());
        userRoles.AddClient(clientId, Defaults);
        userRoles.RemoveClientRole(clientId, "GroupsRole.Owner");

        userRoles.AddClient(clientId, Defaults);

        Assert.Equal(["GroupsRole.Viewer"], Assert.Single(userRoles.Clients).RoleCodes);
    }

    [Fact]
    public void ARevokedClientLosesItsRoles()
    {
        var userRoles = NewUserRoles();
        var clientId = new UserClientId(Guid.NewGuid());
        userRoles.AddClient(clientId, Defaults);

        userRoles.RemoveClient(clientId);
        userRoles.RemoveClient(new UserClientId(Guid.NewGuid()));

        Assert.Empty(userRoles.Clients);
    }

    [Fact]
    public void AnAccountRoleIsAddedAndRemoved()
    {
        var userRoles = NewUserRoles();

        userRoles.AddAccountRole("AuthorizationRole.Owner", Checker);
        Assert.Contains("AuthorizationRole.Owner", userRoles.AccountRoleCodes);
        userRoles.RemoveAccountRole("AuthorizationRole.Owner");

        Assert.DoesNotContain("AuthorizationRole.Owner", userRoles.AccountRoleCodes);
        Assert.Collection(userRoles.DomainEvents,
            e => Assert.Equal("AuthorizationRole.Owner", Assert.IsType<AccountRoleAddedDomainEvent>(e).RoleCode),
            e => Assert.Equal("AuthorizationRole.Owner", Assert.IsType<AccountRoleRemovedDomainEvent>(e).RoleCode));
    }

    [Fact]
    public void ARoleMustExistAndIsGivenOnce()
    {
        var userRoles = NewUserRoles();
        var clientId = new UserClientId(Guid.NewGuid());
        userRoles.AddClient(clientId, Defaults);

        AssertBroken<RoleMustExistRule>(() => userRoles.AddAccountRole("GroupsRole.Admin", Checker));
        AssertBroken<RoleCannotBeAssignedTwiceRule>(() => userRoles.AddAccountRole("GroupsRole.Owner", Checker));
        AssertBroken<RoleMustExistRule>(() => userRoles.AddClientRole(clientId, "GroupsRole.Admin", Checker));
        AssertBroken<RoleCannotBeAssignedTwiceRule>(() => userRoles.AddClientRole(clientId, "GroupsRole.Viewer", Checker));
    }

    [Fact]
    public void OnlyAnAssignedRoleCanBeRemoved()
    {
        var userRoles = NewUserRoles();
        var clientId = new UserClientId(Guid.NewGuid());
        userRoles.AddClient(clientId, Defaults);

        AssertBroken<RoleMustBeAssignedRule>(() => userRoles.RemoveAccountRole("AuthorizationRole.Owner"));
        AssertBroken<RoleMustBeAssignedRule>(() => userRoles.RemoveClientRole(clientId, "AuthorizationRole.Owner"));
    }

    [Fact]
    public void TheRolesOfAClientChangeOnlyThroughItsAccount()
    {
        var userRoles = NewUserRoles();
        var clientId = new UserClientId(Guid.NewGuid());
        userRoles.AddClient(clientId, Defaults);

        userRoles.AddClientRole(clientId, "AuthorizationRole.Owner", Checker);

        Assert.Contains("AuthorizationRole.Owner", Assert.Single(userRoles.Clients).RoleCodes);
        Assert.DoesNotContain("AuthorizationRole.Owner", userRoles.AccountRoleCodes);
        var added = Assert.IsType<ClientRoleAddedDomainEvent>(Assert.Single(userRoles.DomainEvents));
        Assert.Equal(clientId, added.ClientId);

        var otherClient = new UserClientId(Guid.NewGuid());
        AssertBroken<ClientMustBelongToUserRule>(() => userRoles.AddClientRole(otherClient, "GroupsRole.Owner", Checker));
        AssertBroken<ClientMustBelongToUserRule>(() => userRoles.RemoveClientRole(otherClient, "GroupsRole.Owner"));
    }

    [Fact]
    public void ARoleNoModuleDeclaresDisappearsFromTheAccountAndItsClients()
    {
        var userRoles = NewUserRoles();
        userRoles.AddClient(new UserClientId(Guid.NewGuid()), Defaults);

        userRoles.ForgetRole("GroupsRole.Owner");

        Assert.Equal(["GroupsRole.Viewer"], userRoles.AccountRoleCodes);
        Assert.Equal(["GroupsRole.Viewer"], Assert.Single(userRoles.Clients).RoleCodes);
    }
}

public class FakeRoleExistenceChecker(params string[] roleCodes) : IRoleExistenceChecker
{
    public bool Exists(string roleCode) => roleCodes.Contains(roleCode);
}
