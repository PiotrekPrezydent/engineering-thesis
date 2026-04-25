using Dara.Shared.Common.Console;
using Dara.Shared.Contracts.Clients;
using Dara.Shared.Contracts.Common;
using Dara.Shared.Contracts.Interactions;
using Microsoft.AspNetCore.SignalR.Client;
using TypedSignalR.Client;

namespace Dara.Apps.Clients.CLI.ConsoleCommands
{
    public class ConnectionCommands
    {
        private readonly IConnectionInteractions _proxy;
    
        public ConnectionCommands(HubConnection connection)
        {
            _proxy = connection.CreateHubProxy<IConnectionInteractions>();
        }
    
        [ConsoleCommand("activate", "act")]
        async Task ActivateClient(string name, string authToken)
        {
            ClientActivationDto dto = new(name, authToken);
            var response = await _proxy.ActiveClientAsync(dto);
        
            Console.WriteLine($"Response from client: {response.Status} ::: {response.ResponseMessage}");
        }
    
        [ConsoleCommand("deactivate", "dea")]
        async Task DeactivateClient()
        {
            var response = await _proxy.DeactiveClientAsync();
            Console.WriteLine($"Response from client: {response.Status} ::: {response.ResponseMessage}");
        }
    }
}