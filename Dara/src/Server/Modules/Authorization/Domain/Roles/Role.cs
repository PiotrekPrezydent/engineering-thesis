using Dara.Server.BuildingBlocks.Domain;

namespace Dara.Server.Modules.Authorization.Domain.Roles;

// a role a module declares, e.g. "GroupsRole.Owner", with the codes of the permissions of that module it allows;
// read only: only the synchronization at start-up adds, changes and removes roles, the code is the natural key;
// a default role is given to every new account and every new client
public class Role : Entity, IAggregateRoot
{
    private List<string> _permissionCodes = new();

    public RoleId Id { get; private set; }
    public string Code { get; private set; }
    public string Module { get; private set; }
    public bool IsDefault { get; private set; }
    public IReadOnlyList<string> PermissionCodes => _permissionCodes.AsReadOnly();

    private Role() { }

    private Role(string code, string module, List<string> permissionCodes, bool isDefault)
    {
        Id = new RoleId(Guid.NewGuid());
        Code = code;
        Module = module;
        _permissionCodes = permissionCodes;
        IsDefault = isDefault;
    }

    public static Role Register(string code, string module, IEnumerable<string> permissionCodes, bool isDefault)
    {
        return new Role(code, module, Normalize(permissionCodes), isDefault);
    }

    // false when the module declares the role as it is
    public bool Synchronize(IEnumerable<string> permissionCodes, bool isDefault)
    {
        var codes = Normalize(permissionCodes);
        if (IsDefault == isDefault && codes.SequenceEqual(_permissionCodes))
            return false;

        _permissionCodes = codes;
        IsDefault = isDefault;
        return true;
    }

    private static List<string> Normalize(IEnumerable<string> permissionCodes)
    {
        return permissionCodes.Distinct().Order(StringComparer.Ordinal).ToList();
    }
}
