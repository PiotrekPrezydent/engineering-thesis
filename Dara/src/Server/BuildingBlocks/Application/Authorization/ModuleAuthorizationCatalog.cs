namespace Dara.Server.BuildingBlocks.Application.Authorization;

// the permissions and roles of one module, checked at start-up; the host gives the catalogs of all modules to Authorization
public sealed record ModuleAuthorizationCatalog(string Module, IReadOnlyList<string> PermissionCodes, IReadOnlyList<ModuleRoleDefinition> Roles)
{
    public static ModuleAuthorizationCatalog FromAssembly(string module, System.Reflection.Assembly applicationAssembly) =>
        FromTypes(module, applicationAssembly.GetTypes());

    // every permission of the module is in at least one role, a role allows only permissions of its module,
    // role codes are unique in the module and no role is empty; otherwise the module does not start
    public static ModuleAuthorizationCatalog FromTypes(string module, IEnumerable<Type> applicationTypes)
    {
        var types = applicationTypes.Where(t => t.IsClass && !t.IsAbstract).ToList();

        var permissionTypes = types.Where(t => typeof(IModulePermission).IsAssignableFrom(t)).ToHashSet();
        var permissionCodes = permissionTypes
            .SelectMany(ModulePermissions.All)
            .Select(p => p.PermissionCode)
            .OrderBy(c => c, StringComparer.Ordinal)
            .ToList();

        var rolesTypes = types.Where(t => typeof(IModuleRoles).IsAssignableFrom(t)).ToList();
        if (rolesTypes.Count != 1)
            throw new InvalidOperationException(
                $"module {module} must have exactly one implementation of {nameof(IModuleRoles)}, found {rolesTypes.Count}" +
                (rolesTypes.Count > 1 ? $": {string.Join(", ", rolesTypes.Select(t => t.Name))}" : ""));

        var builder = new ModuleRolesBuilder();
        var moduleRoles = (IModuleRoles)Activator.CreateInstance(rolesTypes[0], nonPublic: true)!;
        moduleRoles.Configure(builder);

        var errors = new List<string>();
        var roles = new List<ModuleRoleDefinition>();
        foreach (var role in builder.Roles)
        {
            var roleCode = role.EntityRole.RoleCode;
            if (roles.Any(r => r.RoleCode == roleCode))
                errors.Add($"role {roleCode} is defined more than once");

            if (role.Permissions.Count == 0)
                errors.Add($"role {roleCode} allows no permission");

            var foreign = role.Permissions.Where(p => !permissionTypes.Contains(p.GetType())).Select(p => p.PermissionCode).Distinct().ToList();
            if (foreign.Count > 0)
                errors.Add($"role {roleCode} allows permissions of another module: {string.Join(", ", foreign)}");

            var codes = role.Permissions.Select(p => p.PermissionCode).Distinct().OrderBy(c => c, StringComparer.Ordinal).ToList();
            roles.Add(new ModuleRoleDefinition(roleCode, role.IsDefault, codes));
        }

        var allowed = roles.SelectMany(r => r.PermissionCodes).ToHashSet();
        var uncovered = permissionCodes.Where(c => !allowed.Contains(c)).ToList();
        if (uncovered.Count > 0)
            errors.Add($"permissions that no role allows: {string.Join(", ", uncovered)}");

        if (errors.Count > 0)
            throw new InvalidOperationException($"module {module} has invalid permissions or roles: {string.Join("; ", errors)}");

        return new ModuleAuthorizationCatalog(module, permissionCodes, roles);
    }
}

public sealed record ModuleRoleDefinition(string RoleCode, bool IsDefault, IReadOnlyList<string> PermissionCodes);
