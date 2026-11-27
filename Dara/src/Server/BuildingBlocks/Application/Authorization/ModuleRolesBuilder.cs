using Dara.Server.BuildingBlocks.Domain.Roles;

namespace Dara.Server.BuildingBlocks.Application.Authorization;

// roles.Role(new GroupsRole(GroupsRole.Roles.Owner), isDefault: true).AllowsAll<GroupsPermission>()
//      .Role(new GroupsRole(GroupsRole.Roles.Viewer), isDefault: true).Allows(new GroupsPermission(GroupsPermission.Permissions.ReadGroups));
// a default role is given to every new account and every new client
public sealed class ModuleRolesBuilder
{
    private readonly List<RoleBuilder> _roles = [];

    internal IReadOnlyList<RoleBuilder> Roles => _roles;

    public RoleBuilder Role(IEntityRole role, bool isDefault = false)
    {
        var builder = new RoleBuilder(this, role, isDefault);
        _roles.Add(builder);
        return builder;
    }

    public sealed class RoleBuilder
    {
        private readonly ModuleRolesBuilder _roles;
        private readonly List<IModulePermission> _permissions = [];

        internal IEntityRole EntityRole { get; }
        internal bool IsDefault { get; }
        internal IReadOnlyList<IModulePermission> Permissions => _permissions;

        internal RoleBuilder(ModuleRolesBuilder roles, IEntityRole role, bool isDefault)
        {
            _roles = roles;
            EntityRole = role;
            IsDefault = isDefault;
        }

        public RoleBuilder Allows(params IModulePermission[] permissions)
        {
            _permissions.AddRange(permissions);
            return this;
        }

        // every permission of the permission type of the module
        public RoleBuilder AllowsAll<TPermission>() where TPermission : IModulePermission
        {
            _permissions.AddRange(ModulePermissions.All(typeof(TPermission)));
            return this;
        }

        public RoleBuilder Role(IEntityRole role, bool isDefault = false) => _roles.Role(role, isDefault);
    }
}
