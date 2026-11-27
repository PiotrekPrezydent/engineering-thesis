using Dara.Server.BuildingBlocks.Domain;
using Dara.Server.Modules.Authorization.Domain.Roles;
using Dara.Server.Modules.Authorization.Domain.Users.Clients;
using Dara.Server.Modules.Authorization.Domain.Users.Events;
using Dara.Server.Modules.Authorization.Domain.Users.Rules;

namespace Dara.Server.Modules.Authorization.Domain.Users;

// the roles of one account: the account roles count for the panel (cookie), the roles of a client for that client
// (its token); the clients are entities of this aggregate and change only through the public methods here, each of
// which first checks that the client belongs to the account; created from the events of Identity
public class UserRoles : Entity, IAggregateRoot
{
    private List<string> _accountRoleCodes = new();
    private readonly List<ClientRoles> _clients = new();

    public UserRolesId Id { get; private set; }
    public UserId UserId { get; private set; }
    public IReadOnlyList<string> AccountRoleCodes => _accountRoleCodes.AsReadOnly();
    public IReadOnlyList<ClientRoles> Clients => _clients.AsReadOnly();

    private UserRoles() { }

    private UserRoles(UserId userId, IEnumerable<string> defaultRoleCodes)
    {
        Id = new UserRolesId(Guid.NewGuid());
        UserId = userId;
        _accountRoleCodes = defaultRoleCodes.Distinct().ToList();
    }

    // a new account gets the default roles
    public static UserRoles Create(UserId userId, IEnumerable<string> defaultRoleCodes)
    {
        return new UserRoles(userId, defaultRoleCodes);
    }

    // a new client gets the default roles; a client already known is left as it is (the event came again)
    public void AddClient(UserClientId clientId, IEnumerable<string> defaultRoleCodes)
    {
        if (_clients.Any(c => c.ClientId == clientId))
            return;

        _clients.Add(ClientRoles.Create(clientId, defaultRoleCodes));
    }

    // a revoked client loses its roles; an unknown client is nothing to remove
    public void RemoveClient(UserClientId clientId)
    {
        _clients.RemoveAll(c => c.ClientId == clientId);
    }

    public void AddAccountRole(string roleCode, IRoleExistenceChecker checker)
    {
        CheckRule(new RoleMustExistRule(roleCode, checker));
        CheckRule(new RoleCannotBeAssignedTwiceRule(_accountRoleCodes.Contains(roleCode)));

        _accountRoleCodes.Add(roleCode);
        AddDomainEvent(new AccountRoleAddedDomainEvent(UserId, roleCode));
    }

    public void RemoveAccountRole(string roleCode)
    {
        CheckRule(new RoleMustBeAssignedRule(_accountRoleCodes.Contains(roleCode)));

        _accountRoleCodes.Remove(roleCode);
        AddDomainEvent(new AccountRoleRemovedDomainEvent(UserId, roleCode));
    }

    public void AddClientRole(UserClientId clientId, string roleCode, IRoleExistenceChecker checker)
    {
        GetClient(clientId).AddRole(roleCode, checker);
        AddDomainEvent(new ClientRoleAddedDomainEvent(UserId, clientId, roleCode));
    }

    public void RemoveClientRole(UserClientId clientId, string roleCode)
    {
        GetClient(clientId).RemoveRole(roleCode);
        AddDomainEvent(new ClientRoleRemovedDomainEvent(UserId, clientId, roleCode));
    }

    // a role no module declares any more disappears from the account and its clients
    public void ForgetRole(string roleCode)
    {
        _accountRoleCodes.Remove(roleCode);
        foreach (var client in _clients)
            client.ForgetRole(roleCode);
    }

    private ClientRoles GetClient(UserClientId clientId)
    {
        var client = _clients.FirstOrDefault(c => c.ClientId == clientId);
        CheckRule(new ClientMustBelongToUserRule(client is not null));
        return client!;
    }
}
