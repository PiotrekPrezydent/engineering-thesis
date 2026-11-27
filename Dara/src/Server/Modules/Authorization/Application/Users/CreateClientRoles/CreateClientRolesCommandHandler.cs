using Dara.Server.BuildingBlocks.Application.Commands;
using Dara.Server.Modules.Authorization.Domain.Roles;
using Dara.Server.Modules.Authorization.Domain.Users;
using Dara.Server.Modules.Authorization.Domain.Users.Clients;

namespace Dara.Server.Modules.Authorization.Application.Users.CreateClientRoles;

public class CreateClientRolesCommandHandler : ICommandHandler<CreateClientRolesCommand>
{
    private readonly IUserRolesRepository _userRolesRepository;
    private readonly IRoleRepository _roleRepository;

    public CreateClientRolesCommandHandler(IUserRolesRepository userRolesRepository, IRoleRepository roleRepository)
    {
        _userRolesRepository = userRolesRepository;
        _roleRepository = roleRepository;
    }

    public async Task HandleAsync(CreateClientRolesCommand command)
    {
        var userRoles = await _userRolesRepository.GetOrCreateAsync(_roleRepository, new UserId(command.UserId));
        userRoles.AddClient(new UserClientId(command.ClientId), await _roleRepository.GetDefaultRoleCodesAsync());
    }
}
