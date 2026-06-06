using Dara.Shared.Common.CLI;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Extensions.Logging;

namespace Dara.Clients.Apps.CLI;

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
        var myClientId = Guid.NewGuid().ToString();
        
        var builder = new HubConnectionBuilder();
        Console.WriteLine(myClientId);
        builder.WithUrl(_serverUrl, options => options.Headers.Add("X-Client-Id", myClientId));
        builder.WithAutomaticReconnect();
    
        Connection = builder.Build();
    }


    [CLICommand("connect","con")]
    async Task Connect()
    {
        CLIClient.Logger.LogInformation("Connecting to {url}...", _serverUrl);
        
        await Connection.StartAsync();
        
        Console.WriteLine($"Connected to {_serverUrl}");
    }

    [CLICommand("disconnect", "dis")]
    async Task Disconnect()
    {
        Console.WriteLine($"Disconnecting from {_serverUrl}...");
    
        await Connection.StopAsync();
    
        Console.WriteLine($"Disconnected from {_serverUrl}");
    }
}
