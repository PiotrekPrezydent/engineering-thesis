using Dara.Core.Shared.Services.Auth;
using Dara.Nodes.CLI.ConsoleCommands;

namespace Dara.Nodes.CLI.Services;

public class AuthClient : IAuthService
{
    public Task<LoginResponse> LoginAsync(LoginRequest request)
    {
        Console.WriteLine($"{request.Email}:{request.Password}");
        return null;
    }

    public Task<RegisterResponse> RegisterAsync(RegisterRequest request)
    {
        throw new NotImplementedException();
    }

    public Task<LogoutRequest> LogoutAsync(LogoutRequest request)
    {
        throw new NotImplementedException();
    }
    
    [ConsoleCommand("login")]
    void Login(string username, string password)
    {
        
    }
}

