using Dara.BuildingBlocks.Domain.Commands;
using Dara.Modules.AccessManagment.Domain.Clients;
using Dara.Modules.AccessManagment.Domain.Users;

namespace Dara.Modules.AccessManagment.Application.Clients;

public record LoginClientUserCommand(Guid clientId, Guid userId) : IApplicationCommand;

public record LoginClientUserCommandResult() : IApplicationCommandResult;

public class LoginClientUserCommandHandler : IApplicationCommandHandler<LoginClientUserCommand, LoginClientUserCommandResult>
{
    private readonly IUserRepository _userRepository;
    private readonly IClientRepository _clientRepository;

    public LoginClientUserCommandHandler(IUserRepository userRepository, IClientRepository clientRepository)
    {
        _userRepository = userRepository;
        _clientRepository = clientRepository;
    }
    
    
    public async Task<LoginClientUserCommandResult> HandleAsync(LoginClientUserCommand command)
    {
        var user = await _userRepository.FindById(command.userId);
        var client = await _clientRepository.FindById(command.clientId);
        
        client.AssingUser(user);
        user.AddClient(client);
        
        //clear domain events?
        _userRepository.Save(user);
        _clientRepository.Save(client);
        
        return new LoginClientUserCommandResult();
    }
}