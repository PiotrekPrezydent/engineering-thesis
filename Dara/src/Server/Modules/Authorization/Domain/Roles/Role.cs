using Dara.Server.BuildingBlocks.Domain;

namespace Dara.Server.Modules.Authorization.Domain.Roles;

// a role a module declares, e.g. "Plugins.Editor", with its scope, the codes of every permission it allows (its own
// and those of its subordinate roles of the same scope, already flattened by the synchronization, so no list of roles
// above or below), the codes of the roles its holder may grant and revoke on the same resource, and the codes of its
// companion roles (subordinates of a child scope, passed along with their own resource ids when the role is granted
// or revoked through the api); read only: only the synchronization at start-up adds, changes and removes roles
public class Role : Entity, IAggregateRoot
{
    private List<string> _permissionCodes = new();
    private List<string> _manageableRoleCodes = new();
    private List<string> _companionRoleCodes = new();

    public RoleId Id { get; private set; }
    public string Code { get; private set; }
    public string ScopeCode { get; private set; }
    public string Module { get; private set; }
    public IReadOnlyList<string> PermissionCodes => _permissionCodes.AsReadOnly();
    public IReadOnlyList<string> ManageableRoleCodes => _manageableRoleCodes.AsReadOnly();
    public IReadOnlyList<string> CompanionRoleCodes => _companionRoleCodes.AsReadOnly();

    private Role() { }

    private Role(string code, string scopeCode, string module, List<string> permissionCodes, List<string> manageableRoleCodes,
        List<string> companionRoleCodes)
    {
        Id = new RoleId(Guid.NewGuid());
        Code = code;
        ScopeCode = scopeCode;
        Module = module;
        _permissionCodes = permissionCodes;
        _manageableRoleCodes = manageableRoleCodes;
        _companionRoleCodes = companionRoleCodes;
    }

    public static Role Register(string code, string scopeCode, string module, IEnumerable<string> permissionCodes,
        IEnumerable<string>? manageableRoleCodes = null, IEnumerable<string>? companionRoleCodes = null)
    {
        return new Role(code, scopeCode, module, Normalize(permissionCodes), Normalize(manageableRoleCodes ?? []),
            Normalize(companionRoleCodes ?? []));
    }

    // false when the module declares the role as it is
    public bool Synchronize(string scopeCode, string module, IEnumerable<string> permissionCodes,
        IEnumerable<string>? manageableRoleCodes = null, IEnumerable<string>? companionRoleCodes = null)
    {
        var permissions = Normalize(permissionCodes);
        var manageable = Normalize(manageableRoleCodes ?? []);
        var companions = Normalize(companionRoleCodes ?? []);
        if (ScopeCode == scopeCode && Module == module && permissions.SequenceEqual(_permissionCodes)
            && manageable.SequenceEqual(_manageableRoleCodes) && companions.SequenceEqual(_companionRoleCodes))
            return false;

        ScopeCode = scopeCode;
        Module = module;
        _permissionCodes = permissions;
        _manageableRoleCodes = manageable;
        _companionRoleCodes = companions;
        return true;
    }

    private static List<string> Normalize(IEnumerable<string> codes)
    {
        return codes.Distinct().Order(StringComparer.Ordinal).ToList();
    }
}
