using Dara.Server.BuildingBlocks.Application.Commands;
using Dara.Server.Modules.Authorization.Domain.Permissions;
using Dara.Server.Modules.Authorization.Domain.Roles;
using Dara.Server.Modules.Authorization.Domain.Users;

namespace Dara.Server.Modules.Authorization.Application.Roles.SynchronizePermissionsAndRoles;

// adds what the modules declare anew, changes changed roles and removes what no module declares any more;
// a removed role disappears from the accounts and clients that had it; a second start with the same modules changes nothing
public class SynchronizePermissionsAndRolesCommandHandler : ICommandHandler<SynchronizePermissionsAndRolesCommand>
{
    private readonly IPermissionRepository _permissionRepository;
    private readonly IRoleRepository _roleRepository;
    private readonly IUserRolesRepository _userRolesRepository;

    public SynchronizePermissionsAndRolesCommandHandler(IPermissionRepository permissionRepository, IRoleRepository roleRepository, IUserRolesRepository userRolesRepository)
    {
        _permissionRepository = permissionRepository;
        _roleRepository = roleRepository;
        _userRolesRepository = userRolesRepository;
    }

    public async Task HandleAsync(SynchronizePermissionsAndRolesCommand command)
    {
        await SynchronizePermissionsAsync(command);
        var removedRoleCodes = await SynchronizeRolesAsync(command);
        if (removedRoleCodes.Count == 0)
            return;

        // rare (a module dropped a role), so every account is read once
        foreach (var userRoles in await _userRolesRepository.GetAllAsync())
        {
            foreach (var roleCode in removedRoleCodes)
                userRoles.ForgetRole(roleCode);
        }
    }

    private async Task SynchronizePermissionsAsync(SynchronizePermissionsAndRolesCommand command)
    {
        var declared = command.Catalogs
            .SelectMany(c => c.PermissionCodes.Select(code => (Code: code, c.Module)))
            .ToDictionary(p => p.Code, p => p.Module);
        var existing = await _permissionRepository.GetAllAsync();

        foreach (var permission in existing.Where(p => !declared.ContainsKey(p.Code)))
            _permissionRepository.Remove(permission);

        var existingCodes = existing.Select(p => p.Code).ToHashSet();
        foreach (var (code, module) in declared.Where(p => !existingCodes.Contains(p.Key)))
            await _permissionRepository.AddAsync(Permission.Register(code, module));
    }

    private async Task<List<string>> SynchronizeRolesAsync(SynchronizePermissionsAndRolesCommand command)
    {
        var declared = command.Catalogs
            .SelectMany(c => c.Roles.Select(r => (Role: r, c.Module)))
            .ToDictionary(r => r.Role.RoleCode);
        var existing = (await _roleRepository.GetAllAsync()).ToDictionary(r => r.Code);

        var removed = existing.Values.Where(r => !declared.ContainsKey(r.Code)).ToList();
        foreach (var role in removed)
            _roleRepository.Remove(role);

        foreach (var (code, (definition, module)) in declared)
        {
            if (existing.TryGetValue(code, out var role))
                role.Synchronize(definition.PermissionCodes, definition.IsDefault);
            else
                await _roleRepository.AddAsync(Role.Register(code, module, definition.PermissionCodes, definition.IsDefault));
        }

        return removed.Select(r => r.Code).ToList();
    }
}
