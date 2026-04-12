using Dara.Core.Shared.Dto;

namespace Dara.Core.Shared.Auth;


public interface IAuthHub
{
    public Task Login(UserCredentialsDto userCredentials);
    
    public Task Register(UserDataDto userDataDto);
    
    public Task Logout(Guid userId);
}

public interface IAuthHubClient : IHubClient
{
    public Task ReceiveUserId(Guid userId);
    
}
