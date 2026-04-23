using Dara.Shared.Common.Console;
using Dara.Shared.Contracts.Common;
using Dara.Shared.Contracts.Interactions;
using Microsoft.AspNetCore.SignalR.Client;
using TypedSignalR.Client;

namespace Dara.Apps.Clients.CLI.ConsoleCommands;

public class GuestCommands : IGuestClient
{
    private readonly IGuestInteractions _proxy;
    
    public GuestCommands(HubConnection connecction)
    {
        _proxy = connecction.CreateHubProxy<IGuestInteractions>();
    }
    
    public async Task ReceiveConnectionIpAsync()
    {
        Console.WriteLine("1");
    }

    public async Task ReceiveConnectionIdAsync()
    {
        Console.WriteLine("2");
    }

    public async Task ReceiveAllConnectionsFromIpAsync()
    {
        Console.WriteLine("3");
    }
    
    [ConsoleCommand("enablenode", "en")]
    async Task EnableNode(string name, string authToken)
    {
        Console.WriteLine("enable start");
        NodeDto dto = new(name, authToken);
        var response = await _proxy.EnableClientNodeAsync(dto);
        Console.WriteLine("enable: " + response);
    }
    
    [ConsoleCommand("disablenode", "disable", "disa")]
    async Task DisableNode(string name, string authToken)
    {
        Console.WriteLine("disable start");

        var response = await _proxy.DisableClientNodeAsync();
        Console.WriteLine("disable: " + response);
    }
}