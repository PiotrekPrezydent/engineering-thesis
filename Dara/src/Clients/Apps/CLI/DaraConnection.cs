using Dara.Shared.Common.CLI;
using Dara.Shared.Contracts;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Extensions.Logging;
using TypedSignalR.Client;

namespace Dara.Clients.Apps.CLI;

public class DaraConnection
{
    public const string ServerUrl = "http://127.0.0.1:5273";

    private readonly string _hubUrl = $"{ServerUrl}/{Connections.HubName}";
    private readonly AuthSession _auth;
    private bool _stopRequested;

    public HubConnection? Connection { get; private set; }

    public DaraConnection(AuthSession auth)
    {
        _auth = auth;
    }

    // every (re)connect asks the session for a client token, so an expired one is refreshed first
    public HubConnection SetConnection()
    {
        var builder = new HubConnectionBuilder();
        builder.WithUrl(_hubUrl, options =>
        {
            options.AccessTokenProvider = _auth.GetAccessTokenAsync;
        });
        builder.WithAutomaticReconnect();
        var connection = builder.Build();
        connection.Register<IAppHubClient>(new HubEvents());

        // the server closes the connection when the token expires; when the automatic reconnect does not run,
        // the connection starts again with a refreshed token
        connection.Closed += async _ =>
        {
            if (_stopRequested || !_auth.IsSignedIn)
                return;

            CLIClient.Logger.LogInformation("Connection closed by the server, connecting again...");
            await Task.Delay(TimeSpan.FromSeconds(1));
            try
            {
                await connection.StartAsync();
            }
            catch (Exception e)
            {
                CLIClient.Logger.LogWarning("Could not connect again: {Message}", e.Message);
            }
        };

        Connection = connection;
        return connection;
    }

    [CLICommand("connect","con")]
    async Task Connect()
    {
        if (!_auth.IsSignedIn)
        {
            Console.WriteLine("Sign in first: login <code> (the code comes from the account panel)");
            return;
        }

        CLIClient.Logger.LogInformation("Connecting to {url}...", _hubUrl);

        _stopRequested = false;
        await SetConnection().StartAsync();

        Console.WriteLine($"Connected to {_hubUrl}");
    }

    [CLICommand("disconnect", "dis")]
    async Task Disconnect()
    {
        if (Connection == null)
            return;

        Console.WriteLine($"Disconnecting from {_hubUrl}...");

        _stopRequested = true;
        await Connection.StopAsync();

        Console.WriteLine($"Disconnected from {_hubUrl}");
    }
}
