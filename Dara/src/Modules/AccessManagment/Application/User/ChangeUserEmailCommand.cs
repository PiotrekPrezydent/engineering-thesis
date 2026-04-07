using Dara.Core.Domain.Commands;
using Dara.Modules.AccessManagment.Domain.User;

namespace Dara.Modules.AccessManagment.Application.User;

public record ChangeUserEmailCommand(Guid userId, string newEmail) : IApplicationCommand;

class ChangeUserEmailCommandHandler : IApplicationCommandHandler<ChangeUserEmailCommand>
{
    private readonly IUserRepository _users;
    
    public ChangeUserEmailCommandHandler(IUserRepository users)
    {
        _users = users;
    }
    
    public Task HandleAsync(ChangeUserEmailCommand command)
    {
        UserEmail email = new UserEmail(command.newEmail);
        
        var user = _users.GetUserById(command.userId).Result;
        
        user.ChangeEmail(email);
        return _users.Save(user);
    }
}