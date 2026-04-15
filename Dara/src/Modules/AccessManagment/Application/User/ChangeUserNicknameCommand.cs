using Dara.BuildingBlocks.Domain.Commands;
using Dara.Modules.AccessManagment.Domain.User;

namespace Dara.Modules.AccessManagment.Application.User;

public record ChangeUserNicknameCommand(Guid userId, string newNickname) : IApplicationCommand;

class ChangeUserNicknameCommandHandler : IApplicationCommandHandler<ChangeUserNicknameCommand>
{
    private readonly IUserRepository _users;
    
    public ChangeUserNicknameCommandHandler(IUserRepository users)
    {
        _users = users;
    }
    
    public Task HandleAsync(ChangeUserNicknameCommand command)
    {
        UserNickname nickname = new(command.newNickname);
        
        var user = _users.GetUserById(command.userId).Result;
        
        user.ChangeNickname(nickname);
        return _users.Save(user);
    }
}