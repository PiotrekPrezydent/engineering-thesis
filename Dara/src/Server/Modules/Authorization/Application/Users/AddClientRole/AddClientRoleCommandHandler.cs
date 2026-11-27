using Dara.Server.BuildingBlocks.Application.Commands;
using Dara.Server.Modules.Authorization.Domain.Roles;
using Dara.Server.Modules.Authorization.Domain.Users;
using Dara.Server.Modules.Authorization.Domain.Users.Clients;

namespace Dara.Server.Modules.Authorization.Application.Users.AddClientRole;

public class AddClientRoleCommandHandler : ICommandHandler<AddClientRoleCommand>
{
    private readonly IUserRolesRepository _userRolesRepository;
    private readonly IRoleRepository _roleRepository;
    private readonly IRoleExistenceChecker _roleExistenceChecker;

    public AddClientRoleCommandHandler(IUserRolesRepository userRolesRepository, IRoleRepository roleRepository, IRoleExistenceChecker roleExistenceChecker)
    {
        _userRolesRepository = userRolesRepository;
        _roleRepository = roleRepository;
        _roleExistenceChecker = roleExistenceChecker;
    }

    public async Task HandleAsync(AddClientRoleCommand command)
    {
        var userRoles = await _userRolesRepository.GetOrCreateAsync(_roleRepository, new UserId(command.UserId));
        userRoles.AddClientRole(new UserClientId(command.ClientId), command.RoleCode, _roleExistenceChecker);
    }
}
