using Dara.Server.BuildingBlocks.Domain;

namespace Dara.Server.Modules.Authorization.Domain.Permissions;

// a permission a module declares, e.g. "plugins.plugin.configure" in scope "Plugins.Plugin"; read only: only the
// synchronization at start-up adds, changes and removes permissions, the code is the natural key; the one that
// manages access in its scope is known by its code ("<scope path>.manage_access")
public class Permission : Entity, IAggregateRoot
{
    public PermissionId Id { get; private set; }
    public string Code { get; private set; }
    public string ScopeCode { get; private set; }
    public string Module { get; private set; }

    private Permission() { }

    private Permission(string code, string scopeCode, string module)
    {
        Id = new PermissionId(Guid.NewGuid());
        Code = code;
        ScopeCode = scopeCode;
        Module = module;
    }

    public static Permission Register(string code, string scopeCode, string module)
    {
        return new Permission(code, scopeCode, module);
    }

    // false when the module declares the permission as it is
    public bool Synchronize(string scopeCode, string module)
    {
        if (ScopeCode == scopeCode && Module == module)
            return false;

        ScopeCode = scopeCode;
        Module = module;
        return true;
    }
}
