using Dara.Core.Domain.Commands;
using Dara.Modules.AccessManagment.Application.Auth;

namespace Dara.Modules.AccessManagment.Tests;

public class AccessModuleTest
{
    private readonly IApplicationCommandDispatcher _dispatcher;
    
    public AccessModuleTest(IApplicationCommandDispatcher dispatcher)
    {
        _dispatcher = dispatcher;
    }

    public async Task RunApp()
    {
        Console.WriteLine("start");
        await _dispatcher.DispatchAsync(new RegisterUserCommand("email", "password", "nickname"));
        Console.WriteLine("koniec");
    }
}