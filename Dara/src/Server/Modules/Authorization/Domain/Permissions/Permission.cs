using Dara.Server.BuildingBlocks.Domain;

namespace Dara.Server.Modules.Authorization.Domain.Permissions;

// a permission a module declares, e.g. "GroupsPermission.SendMessage"; read only: only the synchronization at start-up
// adds a new one and removes one the module no longer declares, the code is the natural key
public class Permission : Entity, IAggregateRoot
{
    public PermissionId Id { get; private set; }
    public string Code { get; private set; }
    public string Module { get; private set; }

    private Permission() { }

    private Permission(string code, string module)
    {
        Id = new PermissionId(Guid.NewGuid());
        Code = code;
        Module = module;
    }

    public static Permission Register(string code, string module)
    {
        return new Permission(code, module);
    }
}
