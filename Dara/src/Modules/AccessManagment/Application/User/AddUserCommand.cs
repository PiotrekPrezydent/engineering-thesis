using Dara.BuildingBlocks.Domain.Commands;
using Dara.Modules.AccessManagment.Domain.Users;

namespace Dara.Modules.AccessManagment.Application.User;

public record AddUserCommand(string Email, string Nickname, string Password) : IApplicationCommand;

public record AddUserCommandResult() : IApplicationCommandResult;

public class AddUserCommandHandler : IApplicationCommandHandler<AddUserCommand, AddUserCommandResult>
{
    private readonly IUserRepository _userRepository;

    public AddUserCommandHandler(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }
    
    public async Task<AddUserCommandResult> HandleAsync(AddUserCommand command)
    {
        UserEmail email = new(command.Email);
        UserNickname nickname = new(command.Nickname);
        UserPassword password = new(command.Password);

        AccessManagment.Domain.Users.User user = new(email, nickname, password);
        
        await _userRepository.Add(user);
        
        return new AddUserCommandResult();
    }
}