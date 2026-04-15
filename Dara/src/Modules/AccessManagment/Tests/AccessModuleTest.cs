using Dara.BuildingBlocks.Domain.Commands;
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
        var test = await _dispatcher.DispatchAsync<RegisterUserCommand, RegisterUserCommandResult>(new RegisterUserCommand("email", "password", "nickname"));
        Console.WriteLine(test.userId + " readed");
        Console.WriteLine("koniec");
    }
}