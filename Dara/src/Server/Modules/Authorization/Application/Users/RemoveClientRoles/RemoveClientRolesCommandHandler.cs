using Dara.Server.BuildingBlocks.Application.Commands;
using Dara.Server.Modules.Authorization.Domain.Roles;
using Dara.Server.Modules.Authorization.Domain.Users;
using Dara.Server.Modules.Authorization.Domain.Users.Clients;

namespace Dara.Server.Modules.Authorization.Application.Users.RemoveClientRoles;

public class RemoveClientRolesCommandHandler : ICommandHandler<RemoveClientRolesCommand>
{
    private readonly IUserRolesRepository _userRolesRepository;
    private readonly IRoleRepository _roleRepository;

    public RemoveClientRolesCommandHandler(IUserRolesRepository userRolesRepository, IRoleRepository roleRepository)
    {
        _userRolesRepository = userRolesRepository;
        _roleRepository = roleRepository;
    }

    public async Task HandleAsync(RemoveClientRolesCommand command)
    {
        var userRoles = await _userRolesRepository.FindByUserIdAsync(new UserId(command.UserId));
        userRoles?.RemoveClient(new UserClientId(command.ClientId));
    }
}
