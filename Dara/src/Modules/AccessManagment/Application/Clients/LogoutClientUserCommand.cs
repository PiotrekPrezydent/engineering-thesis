using Dara.BuildingBlocks.Domain.Commands;
using Dara.Modules.AccessManagment.Domain.Clients;
using Dara.Modules.AccessManagment.Domain.Users;

namespace Dara.Modules.AccessManagment.Application.Clients;

public record LogoutClientUserCommand(Guid ClientId) : IApplicationCommand;

public record LogoutClientUserCommandResult() : IApplicationCommandResult;

public class LogoutClientUserCommandHandler : IApplicationCommandHandler<LogoutClientUserCommand, LogoutClientUserCommandResult>
{
    private readonly IClientRepository _clientRepository;
    private readonly IUserRepository _userRepository;

    public LogoutClientUserCommandHandler(IClientRepository clientRepository, IUserRepository userRepository)
    {
        _clientRepository = clientRepository;
        _userRepository = userRepository;
    }
    
    public async Task<LogoutClientUserCommandResult> HandleAsync(LogoutClientUserCommand command)
    {
        var client = await _clientRepository.FindById(command.ClientId);
        var user = client.User;
        
        client.RemoveUser(user); // why this have user in args
        user.RemoveClient(client);
        
        await _userRepository.Save(user);
        await _clientRepository.Save(client);
        
        return new LogoutClientUserCommandResult();
    }
}