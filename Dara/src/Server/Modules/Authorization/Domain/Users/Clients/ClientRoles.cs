using Dara.Server.BuildingBlocks.Domain;
using Dara.Server.Modules.Authorization.Domain.Roles;
using Dara.Server.Modules.Authorization.Domain.Users.Rules;

namespace Dara.Server.Modules.Authorization.Domain.Users.Clients;

// the roles of one client of the account, they count when the client calls with its token;
// changes only through UserRoles, which first checks that the client belongs to the account
public class ClientRoles : Entity
{
    private List<string> _roleCodes = new();

    public ClientRolesId Id { get; private set; }
    public UserClientId ClientId { get; private set; }
    public IReadOnlyList<string> RoleCodes => _roleCodes.AsReadOnly();

    private ClientRoles() { }

    private ClientRoles(UserClientId clientId, IEnumerable<string> roleCodes)
    {
        Id = new ClientRolesId(Guid.NewGuid());
        ClientId = clientId;
        _roleCodes = roleCodes.Distinct().ToList();
    }

    internal static ClientRoles Create(UserClientId clientId, IEnumerable<string> defaultRoleCodes)
    {
        return new ClientRoles(clientId, defaultRoleCodes);
    }

    internal void AddRole(string roleCode, IRoleExistenceChecker checker)
    {
        CheckRule(new RoleMustExistRule(roleCode, checker));
        CheckRule(new RoleCannotBeAssignedTwiceRule(_roleCodes.Contains(roleCode)));

        _roleCodes.Add(roleCode);
    }

    internal void RemoveRole(string roleCode)
    {
        CheckRule(new RoleMustBeAssignedRule(_roleCodes.Contains(roleCode)));

        _roleCodes.Remove(roleCode);
    }

    // false when the client did not have the role
    internal bool ForgetRole(string roleCode)
    {
        return _roleCodes.Remove(roleCode);
    }
}
