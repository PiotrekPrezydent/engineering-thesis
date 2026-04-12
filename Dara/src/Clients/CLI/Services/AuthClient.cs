using Dara.Core.Shared;
using Dara.Core.Shared.Auth;
using Dara.Core.Shared.Dto;
using Dara.Nodes.CLI.ConsoleCommands;
using Microsoft.AspNetCore.SignalR.Client;
using TypedSignalR.Client;

namespace Dara.Nodes.CLI.Services;

public class AuthClient : IAuthHubClient
{
    HubConnection _connection;
    IAuthHub _hub;

    public AuthClient(HubConnection connection)
    {
        _connection = connection;
        _hub = connection.CreateHubProxy<IAuthHub>();
    }
    
    [ConsoleCommand("login")]
    async Task Login(string email, string password)
    {
        try
        {
            await _hub.Login(new UserCredentialsDto(email, password));
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
        }
        finally
        {
            Console.WriteLine("fin");
        }

    }

    public Task GetMessage(string message)
    {
        Console.WriteLine($"{message}-----------");
        return Task.CompletedTask;
    }

    public Task GetExcetpion(Exception ex)
    {
        Console.WriteLine("called get");
        throw ex;
    }

    public Task ReceiveUserId(Guid userId)
    {
        Console.WriteLine($"{userId}");
        return Task.CompletedTask;
    }
}

