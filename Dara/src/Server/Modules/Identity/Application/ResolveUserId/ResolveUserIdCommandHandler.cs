using Dara.Server.BuildingBlocks.Application.Commands;
using Dara.Server.Modules.Identity.Domain;

namespace Dara.Server.Modules.Identity.Application.ResolveUserId;

public class ResolveUserIdCommandHandler : ICommandHandler<ResolveUserIdCommand, Guid>
{
    private IUserRepository _userRepository;
    
    public ResolveUserIdCommandHandler(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }
    
    public async Task<Guid> HandleAsync(ResolveUserIdCommand command)
    {
        var user = await _userRepository.GetByUserIdentifierAsync(command.UserIdentifier);
        
        if (user is null)
        {
            user = User.CreateNewUser(command.UserIdentifier);
            await _userRepository.AddAsync(user);
        }
        
        return user.UserId.Value;
    }
}