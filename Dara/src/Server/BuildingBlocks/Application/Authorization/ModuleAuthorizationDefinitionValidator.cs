namespace Dara.Server.BuildingBlocks.Application.Authorization;

public static class ModuleAuthorizationDefinitionValidator
{
    // the one declaration of the module in its Application assembly, checked; otherwise the module does not start
    public static IModuleAuthorizationDefinition FromAssembly(string moduleName, System.Reflection.Assembly applicationAssembly)
    {
        var types = applicationAssembly.GetTypes()
            .Where(t => t is { IsClass: true, IsAbstract: false } && typeof(IModuleAuthorizationDefinition).IsAssignableFrom(t))
            .ToList();

        if (types.Count != 1)
            throw new InvalidOperationException(
                $"module {moduleName} must have exactly one implementation of {nameof(IModuleAuthorizationDefinition)}, found {types.Count}" +
                (types.Count > 1 ? $": {string.Join(", ", types.Select(t => t.Name))}" : ""));

        var definition = (IModuleAuthorizationDefinition)Activator.CreateInstance(types[0], nonPublic: true)!;
        Validate(moduleName, definition);
        return definition;
    }

    // codes are unique; scopes and roles start with the name of the module, a permission is "<scope path>.<action>";
    // the permission that manages access in a scope is only "<scope path>.manage_access" (so at most one); a role
    // allows declared permissions of its own scope and at least one (its own, of its subordinates of the same scope,
    // or companion roles); subordinates are declared roles of the same or of a child scope, without cycles; manageable
    // roles are declared roles of the same scope and a role that manages roles has the permission that manages access
    // in its scope; every permission is in a role; new subjects get declared roles
    public static void Validate(string moduleName, IModuleAuthorizationDefinition definition)
    {
        var errors = new List<string>();
        if (!string.Equals(definition.ModuleName, moduleName, StringComparison.Ordinal))
            errors.Add($"the definition names module {definition.ModuleName}");

        var scopes = definition.GetScopes().ToList();
        var permissions = definition.GetPermissions().ToList();
        var roles = definition.GetDefaultRoles().ToList();

        var prefix = moduleName + ".";
        foreach (var code in scopes.Select(s => s.Code).Concat(roles.Select(r => r.Code)).Where(c => !HasPrefix(c, prefix)))
            errors.Add($"code {code} does not start with {prefix}");

        foreach (var permission in permissions)
        {
            var scopePrefix = permission.Scope.Code + ".";
            if (!HasPrefix(permission.Code, scopePrefix) || permission.Code[scopePrefix.Length..].Contains('.'))
                errors.Add($"permission {permission.Code} is not {permission.Scope.Code.ToLowerInvariant()}.<action>");

            if (permission.Code.EndsWith("." + ScopeDefinition.ManageAccessAction, StringComparison.OrdinalIgnoreCase) && !permission.ManagesAccess)
                errors.Add($"permission {permission.Code} would manage access, which in scope {permission.Scope.Code} only {permission.Scope.ManageAccessPermissionCode} does");
        }

        AddDuplicates(errors, "scope", scopes.Select(s => s.Code));
        AddDuplicates(errors, "permission", permissions.Select(p => p.Code));
        AddDuplicates(errors, "role", roles.Select(r => r.Code));

        var scopeCodes = scopes.Select(s => s.Code).ToHashSet(StringComparer.Ordinal);
        foreach (var permission in permissions.Where(p => !scopeCodes.Contains(p.Scope.Code)))
            errors.Add($"permission {permission.Code} is in scope {permission.Scope.Code} the module does not declare");

        var declared = permissions.ToHashSet();
        var rolesByCode = roles.GroupBy(r => r.Code, StringComparer.Ordinal).ToDictionary(g => g.Key, g => g.First(), StringComparer.Ordinal);
        foreach (var role in roles)
            ValidateRole(errors, role, scopeCodes, declared, rolesByCode);

        var allowed = roles.SelectMany(r => r.AllPermissions()).Select(p => p.Code).ToHashSet(StringComparer.Ordinal);
        var uncovered = permissions.Select(p => p.Code).Where(c => !allowed.Contains(c)).ToList();
        if (uncovered.Count > 0)
            errors.Add($"permissions that no role allows: {string.Join(", ", uncovered)}");

        foreach (var role in definition.GetNewUserRoles().Concat(definition.GetNewClientRoles()).Where(r => !rolesByCode.ContainsKey(r.Role.Code)))
            errors.Add($"new subjects get role {role.Role.Code} the module does not declare");

        if (errors.Count > 0)
            throw new InvalidOperationException($"module {moduleName} has an invalid authorization definition: {string.Join("; ", errors)}");
    }

    private static void ValidateRole(List<string> errors, RoleDefinition role, HashSet<string> scopeCodes,
        HashSet<PermissionDefinition> declared, Dictionary<string, RoleDefinition> rolesByCode)
    {
        if (!scopeCodes.Contains(role.Scope.Code))
            errors.Add($"role {role.Code} is in scope {role.Scope.Code} the module does not declare");

        var all = role.AllPermissions();
        if (all.Count == 0 && role.CompanionRoles().Count == 0)
            errors.Add($"role {role.Code} allows no permission");

        var outside = role.Permissions.Where(p => p.Scope.Code != role.Scope.Code).Select(p => p.Code).ToList();
        if (outside.Count > 0)
            errors.Add($"role {role.Code} of scope {role.Scope.Code} allows permissions of another scope: {string.Join(", ", outside)}");

        var undeclared = role.Permissions.Where(p => !declared.Contains(p)).Select(p => p.Code).ToList();
        if (undeclared.Count > 0)
            errors.Add($"role {role.Code} allows permissions the module does not declare: {string.Join(", ", undeclared)}");

        foreach (var subordinate in role.Subordinates)
        {
            if (!rolesByCode.TryGetValue(subordinate.Code, out var related))
                errors.Add($"role {role.Code} has subordinate {subordinate.Code} the module does not declare");
            else if (related.Scope.Code != role.Scope.Code && !related.Scope.Code.StartsWith(role.Scope.Code + ".", StringComparison.Ordinal))
                errors.Add($"role {role.Code} of scope {role.Scope.Code} has subordinate {subordinate.Code} of scope {related.Scope.Code}, neither the same nor a child scope");
        }

        foreach (var code in role.ManageableRoleCodes)
        {
            if (!rolesByCode.TryGetValue(code, out var related))
                errors.Add($"role {role.Code} manages role {code} the module does not declare");
            else if (related.Scope.Code != role.Scope.Code)
                errors.Add($"role {role.Code} of scope {role.Scope.Code} manages role {code} of scope {related.Scope.Code}");
        }

        if (ReachesItself(role))
            errors.Add($"role {role.Code} is its own subordinate");

        if (role.ManageableRoleCodes.Count > 0 && !all.Any(p => p.Scope.Code == role.Scope.Code && p.ManagesAccess))
            errors.Add($"role {role.Code} manages roles without {role.Scope.ManageAccessPermissionCode}");
    }

    private static bool ReachesItself(RoleDefinition role)
    {
        var pending = new Stack<RoleDefinition>(role.Subordinates);
        var seen = new HashSet<string>(StringComparer.Ordinal);
        while (pending.Count > 0)
        {
            var next = pending.Pop();
            if (next.Code == role.Code)
                return true;

            if (!seen.Add(next.Code))
                continue;

            foreach (var subordinate in next.Subordinates)
                pending.Push(subordinate);
        }

        return false;
    }

    private static bool HasPrefix(string code, string prefix)
    {
        return code.StartsWith(prefix, StringComparison.OrdinalIgnoreCase) && code.Length > prefix.Length;
    }

    private static void AddDuplicates(List<string> errors, string kind, IEnumerable<string> codes)
    {
        foreach (var code in codes.GroupBy(c => c, StringComparer.Ordinal).Where(g => g.Count() > 1).Select(g => g.Key))
            errors.Add($"{kind} {code} is declared more than once");
    }
}
