using Dara.Server.BuildingBlocks.Application.Authorization;
using Dara.Server.BuildingBlocks.Application.Commands;
using Dara.Server.Modules.Authorization.Domain.Permissions;
using Dara.Server.Modules.Authorization.Domain.RoleAssignments;
using Dara.Server.Modules.Authorization.Domain.Roles;

namespace Dara.Server.Modules.Authorization.Application.Definitions.SynchronizeDefinitions;

// adds what the modules declare anew, changes what changed and removes what no module declares any more; a role
// keeps every permission it allows (its own and of its subordinates, flattened here) and its manageable roles; the
// assignments of a removed role (or of a role that moved to another scope) disappear; a second start with the same
// modules changes nothing; a code two modules declare stops the start
public class SynchronizeDefinitionsCommandHandler : ICommandHandler<SynchronizeDefinitionsCommand>
{
    private readonly IPermissionRepository _permissionRepository;
    private readonly IRoleRepository _roleRepository;
    private readonly IRoleAssignmentRepository _assignmentRepository;

    public SynchronizeDefinitionsCommandHandler(IPermissionRepository permissionRepository, IRoleRepository roleRepository,
        IRoleAssignmentRepository assignmentRepository)
    {
        _permissionRepository = permissionRepository;
        _roleRepository = roleRepository;
        _assignmentRepository = assignmentRepository;
    }

    public async Task HandleAsync(SynchronizeDefinitionsCommand command)
    {
        var scopes = command.Definitions.SelectMany(d => d.GetScopes().Select(s => (s.Code, d.ModuleName))).ToList();
        var permissions = command.Definitions.SelectMany(d => d.GetPermissions().Select(p => (Definition: p, d.ModuleName))).ToList();
        var roles = command.Definitions.SelectMany(d => d.GetDefaultRoles().Select(r => (Definition: r, d.ModuleName))).ToList();

        var errors = Duplicates("scope", scopes)
            .Concat(Duplicates("permission", permissions.Select(p => (p.Definition.Code, p.ModuleName))))
            .Concat(Duplicates("role", roles.Select(r => (r.Definition.Code, r.ModuleName))))
            .ToList();
        if (errors.Count > 0)
            throw new InvalidOperationException($"authorization definitions of the modules collide: {string.Join("; ", errors)}");

        await SynchronizePermissionsAsync(permissions);
        var withdrawnRoleCodes = await SynchronizeRolesAsync(roles);
        if (withdrawnRoleCodes.Count == 0)
            return;

        foreach (var assignment in await _assignmentRepository.GetWithRolesAsync(withdrawnRoleCodes))
            _assignmentRepository.Remove(assignment);
    }

    private static IEnumerable<string> Duplicates(string kind, IEnumerable<(string Code, string ModuleName)> codes)
    {
        return codes
            .GroupBy(c => c.Code, StringComparer.Ordinal)
            .Where(g => g.Count() > 1)
            .Select(g => $"{kind} {g.Key} is declared by {string.Join(" and ", g.Select(c => c.ModuleName))}");
    }

    private async Task SynchronizePermissionsAsync(List<(PermissionDefinition Definition, string ModuleName)> declared)
    {
        var byCode = declared.ToDictionary(p => p.Definition.Code);
        var existing = await _permissionRepository.GetAllAsync();

        foreach (var permission in existing)
        {
            if (byCode.TryGetValue(permission.Code, out var definition))
                permission.Synchronize(definition.Definition.Scope.Code, definition.ModuleName);
            else
                _permissionRepository.Remove(permission);
        }

        var existingCodes = existing.Select(p => p.Code).ToHashSet();
        foreach (var (definition, module) in declared.Where(p => !existingCodes.Contains(p.Definition.Code)))
            await _permissionRepository.AddAsync(Permission.Register(definition.Code, definition.Scope.Code, module));
    }

    // the codes of the roles whose assignments are no longer valid
    private async Task<List<string>> SynchronizeRolesAsync(List<(RoleDefinition Definition, string ModuleName)> declared)
    {
        var byCode = declared.ToDictionary(r => r.Definition.Code);
        var existing = await _roleRepository.GetAllAsync();
        var withdrawn = new List<string>();

        foreach (var role in existing)
        {
            if (!byCode.TryGetValue(role.Code, out var definition))
            {
                _roleRepository.Remove(role);
                withdrawn.Add(role.Code);
                continue;
            }

            var scopeBefore = role.ScopeCode;
            role.Synchronize(definition.Definition.Scope.Code, definition.ModuleName, PermissionCodes(definition.Definition),
                definition.Definition.ManageableRoleCodes, CompanionCodes(definition.Definition));
            if (scopeBefore != role.ScopeCode)
                withdrawn.Add(role.Code);
        }

        var existingCodes = existing.Select(r => r.Code).ToHashSet();
        foreach (var (definition, module) in declared.Where(r => !existingCodes.Contains(r.Definition.Code)))
            await _roleRepository.AddAsync(Role.Register(definition.Code, definition.Scope.Code, module, PermissionCodes(definition),
                definition.ManageableRoleCodes, CompanionCodes(definition)));

        return withdrawn;
    }

    // its own permissions and those of its subordinates of the same scope
    private static IEnumerable<string> PermissionCodes(RoleDefinition role)
    {
        return role.AllPermissions().Select(p => p.Code);
    }

    // its subordinates of a child scope: never flattened, they hold on resources of their own
    private static IEnumerable<string> CompanionCodes(RoleDefinition role)
    {
        return role.CompanionRoles().Select(r => r.Code);
    }
}
