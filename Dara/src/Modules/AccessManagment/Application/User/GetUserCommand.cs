using Dara.BuildingBlocks.Domain.Commands;
using Dara.Modules.AccessManagment.Domain.Users;

namespace Dara.Modules.AccessManagment.Application.User;

public record GetUserCommand(string Email) : IApplicationCommand;

public record GetUserCommandResult(Guid UserId) : IApplicationCommandResult;

public class GetUserCommandHandler : IApplicationCommandHandler<GetUserCommand, GetUserCommandResult>
{
    private readonly IUserRepository _userRepository;
    
    public GetUserCommandHandler(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }
    
    public async Task<GetUserCommandResult> HandleAsync(GetUserCommand command)
    {
        UserEmail email = new(command.Email);
        
        var user = await _userRepository.FindByEmail(email);
        
        return new GetUserCommandResult(user.Id);
    }
}