using Dara.Shared.Common.Console;
using Microsoft.AspNetCore.SignalR.Client;

namespace Dara.Apps.Clients.CLI;

public class DaraConnection
{
    private readonly string _serverUrl;
    public HubConnection Connection { get; private set; }

    public DaraConnection()
    {
        _serverUrl = ProvideUrl();
        BuildConnection();
    }

    string ProvideUrl()
    {
        return "http://127.0.0.1:5273/app";
    }

    void BuildConnection()
    {
        var builder = new HubConnectionBuilder();
        builder.WithUrl(_serverUrl);
        builder.WithAutomaticReconnect();
        
        Connection = builder.Build();
    }
    

    [ConsoleCommand("connect","con")]
    async Task Connect()
    {
        Console.WriteLine($"Connecting to {_serverUrl}...");
        
        await Connection.StartAsync();
        
        Console.WriteLine($"Connected to {_serverUrl}");
    }

    [ConsoleCommand("disconnect", "disc")]
    async Task Disconnect()
    {
        Console.WriteLine($"Disconnecting from {_serverUrl}...");
        
        await Connection.StopAsync();
        
        Console.WriteLine($"Disconnected from {_serverUrl}");
    }
}