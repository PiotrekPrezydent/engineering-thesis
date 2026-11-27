using Dara.Server.BuildingBlocks.Application.Commands;
using Dara.Server.Modules.Authorization.Domain.Roles;
using Dara.Server.Modules.Authorization.Domain.Users;

namespace Dara.Server.Modules.Authorization.Application.Users.RemoveAccountRole;

public class RemoveAccountRoleCommandHandler : ICommandHandler<RemoveAccountRoleCommand>
{
    private readonly IUserRolesRepository _userRolesRepository;
    private readonly IRoleRepository _roleRepository;

    public RemoveAccountRoleCommandHandler(IUserRolesRepository userRolesRepository, IRoleRepository roleRepository)
    {
        _userRolesRepository = userRolesRepository;
        _roleRepository = roleRepository;
    }

    public async Task HandleAsync(RemoveAccountRoleCommand command)
    {
        var userRoles = await _userRolesRepository.GetOrCreateAsync(_roleRepository, new UserId(command.UserId));
        userRoles.RemoveAccountRole(command.RoleCode);
    }
}
