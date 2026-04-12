using Dara.Core.Domain.Commands;
using Dara.Core.Shared;
using Dara.Core.Shared.Auth;
using Dara.Core.Shared.Dto;
using Dara.Modules.AccessManagment.Application.Auth;

namespace Dara.API.Server.Auth;

public class AuthHub : HubBase<IAuthHubClient>, IAuthHub
{
    public AuthHub(IApplicationCommandDispatcher commandDispatcher) : base(commandDispatcher)
    {
    }

    public async Task Login(UserCredentialsDto userCredentials)
    {
        //await Clients.Caller.GetMessage("test message");
        //await _commandDispatcher.DispatchAsync(new LoginUserCommand(userCredentials.Email, userCredentials.Password));
        await Clients.Caller.GetExcetpion(new Exception("test"));
    }

    public Task Register(UserDataDto userDataDto)
    {
        throw new NotImplementedException();
    }

    public Task Logout(Guid userId)
    {
        throw new NotImplementedException();
    }
}