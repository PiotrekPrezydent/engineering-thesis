using Dara.Server.BuildingBlocks.Application.Commands;
using Dara.Server.Modules.Authorization.Domain.Roles;
using Dara.Server.Modules.Authorization.Domain.Users;

namespace Dara.Server.Modules.Authorization.Application.Users.AddAccountRole;

public class AddAccountRoleCommandHandler : ICommandHandler<AddAccountRoleCommand>
{
    private readonly IUserRolesRepository _userRolesRepository;
    private readonly IRoleRepository _roleRepository;
    private readonly IRoleExistenceChecker _roleExistenceChecker;

    public AddAccountRoleCommandHandler(IUserRolesRepository userRolesRepository, IRoleRepository roleRepository, IRoleExistenceChecker roleExistenceChecker)
    {
        _userRolesRepository = userRolesRepository;
        _roleRepository = roleRepository;
        _roleExistenceChecker = roleExistenceChecker;
    }

    public async Task HandleAsync(AddAccountRoleCommand command)
    {
        var userRoles = await _userRolesRepository.GetOrCreateAsync(_roleRepository, new UserId(command.UserId));
        userRoles.AddAccountRole(command.RoleCode, _roleExistenceChecker);
    }
}
