using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Security.Claims;
using System.Text;
using Dara.Shared.Common.CLI;
using Dara.Shared.Contracts;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;

namespace Dara.Clients.Apps.CLI;

public class DaraConnection
{
    private readonly string _serverUrl;
    public HubConnection Connection { get; private set; }

    public DaraConnection()
    {
        _serverUrl = ProvideUrl();
        
        string identifier = GenerateIdentifier();
        CLIClient.Logger.LogInformation($"Created id {identifier}");
        
        var builder = new HubConnectionBuilder();
        builder.WithUrl(_serverUrl, options =>
        {
            options.Headers[Connections.IdentifierHeaderName] = identifier;
        });
        builder.WithAutomaticReconnect();
    
        Connection = builder.Build();
    }

    string ProvideUrl()
    {
        return $"http://127.0.0.1:5273/{Connections.HubName}";
    }

    string GenerateIdentifier()
    {
        var secretKey = $"{AppDomain.CurrentDomain.BaseDirectory}-{DateTime.UtcNow.Ticks}";
        var key = Encoding.ASCII.GetBytes(secretKey);
        return Convert.ToBase64String(key);
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

    [CLICommand("test")]
    async Task TestCall()
    {
        await Connection.SendAsync("Test");
    }
}
