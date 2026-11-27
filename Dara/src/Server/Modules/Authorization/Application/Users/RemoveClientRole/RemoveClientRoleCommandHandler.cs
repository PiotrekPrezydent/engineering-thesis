using Dara.Server.BuildingBlocks.Application.Commands;
using Dara.Server.Modules.Authorization.Domain.Roles;
using Dara.Server.Modules.Authorization.Domain.Users;
using Dara.Server.Modules.Authorization.Domain.Users.Clients;

namespace Dara.Server.Modules.Authorization.Application.Users.RemoveClientRole;

public class RemoveClientRoleCommandHandler : ICommandHandler<RemoveClientRoleCommand>
{
    private readonly IUserRolesRepository _userRolesRepository;
    private readonly IRoleRepository _roleRepository;

    public RemoveClientRoleCommandHandler(IUserRolesRepository userRolesRepository, IRoleRepository roleRepository)
    {
        _userRolesRepository = userRolesRepository;
        _roleRepository = roleRepository;
    }

    public async Task HandleAsync(RemoveClientRoleCommand command)
    {
        var userRoles = await _userRolesRepository.GetOrCreateAsync(_roleRepository, new UserId(command.UserId));
        userRoles.RemoveClientRole(new UserClientId(command.ClientId), command.RoleCode);
    }
}
