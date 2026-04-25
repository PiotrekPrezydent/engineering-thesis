using Dara.Shared.Common.Console;
using Dara.Shared.Contracts.Clients;
using Dara.Shared.Contracts.Interactions;
using Microsoft.AspNetCore.SignalR.Client;
using TypedSignalR.Client;

namespace Dara.Apps.Clients.CLI.ConsoleCommands;

public class ActiveConnectionCommands : IActiveConnectionClient
{
    private readonly IActiveConnectionInteractions _proxy;
    
    public ActiveConnectionCommands(HubConnection connection)
    {
        _proxy = connection.CreateHubProxy<IActiveConnectionInteractions>();
        connection.Register<IActiveConnectionClient>(this);
    }
    
    [ConsoleCommand("changename", "cn")]
    async Task ChangeName(string name)
    {
        ClientNameDto clientName = new(name);
        var response = await _proxy.ChangeClientNameAsync(clientName);
        
        Console.WriteLine($"Response from client: {response.Status} ::: {response.ResponseMessage}");
    }
    
    [ConsoleCommand("changetoken", "ct")]
    async Task ChangeToken(string token)
    {
        ClientAuthTokenDto clientToken = new(token);
        var response = await _proxy.ChangeClientAuthTokenAsync(clientToken);
        
        Console.WriteLine($"Response from client: {response.Status} ::: {response.ResponseMessage}");
    }
}