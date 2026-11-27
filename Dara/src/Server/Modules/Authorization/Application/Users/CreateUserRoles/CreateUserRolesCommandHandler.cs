using Dara.Server.BuildingBlocks.Application.Commands;
using Dara.Server.Modules.Authorization.Domain.Roles;
using Dara.Server.Modules.Authorization.Domain.Users;

namespace Dara.Server.Modules.Authorization.Application.Users.CreateUserRoles;

public class CreateUserRolesCommandHandler : ICommandHandler<CreateUserRolesCommand>
{
    private readonly IUserRolesRepository _userRolesRepository;
    private readonly IRoleRepository _roleRepository;

    public CreateUserRolesCommandHandler(IUserRolesRepository userRolesRepository, IRoleRepository roleRepository)
    {
        _userRolesRepository = userRolesRepository;
        _roleRepository = roleRepository;
    }

    public async Task HandleAsync(CreateUserRolesCommand command)
    {
        var userId = new UserId(command.UserId);
        if (await _userRolesRepository.FindByUserIdAsync(userId) is not null)
            return;

        await _userRolesRepository.AddAsync(UserRoles.Create(userId, await _roleRepository.GetDefaultRoleCodesAsync()));
    }
}
