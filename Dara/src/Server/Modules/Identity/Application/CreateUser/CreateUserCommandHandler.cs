using Dara.Server.BuildingBlocks.Application.Commands;
using Dara.Server.Modules.Identity.Domain;

namespace Dara.Server.Modules.Identity.Application.CreateUser;

public class CreateUserCommandHandler : ICommandHandler<CreateUserCommand, Guid>
{
    private IUserRepository _userRepository;
    
    public CreateUserCommandHandler(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }
    
    public async Task<Guid> HandleAsync(CreateUserCommand userCommand)
    {
        var client = User.Create(userCommand.UserIdentifier);
        await _userRepository.AddAsync(client);
        
        return client.Id.Value;
    }
}