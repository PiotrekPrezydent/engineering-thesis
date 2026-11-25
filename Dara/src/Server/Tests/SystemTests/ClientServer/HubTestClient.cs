using Dara.Shared.Contracts;
using Microsoft.AspNetCore.SignalR.Client;
using TypedSignalR.Client;

namespace Dara.Server.SystemTests.ClientServer;

// one device built like DaraConnection of the cli: the identifier header, typed hub proxy and typed receiver
public class HubTestClient : IAsyncDisposable
{
    private readonly string _hubUrl;
    private HubConnection _connection;

    public string Identifier { get; }
    public Guid UserId { get; set; }
    public IAppHub Hub { get; private set; } = null!;

    // kept across connections, so a test sees everything the device got
    public RecordingHubClient Events { get; } = new();

    public HubTestClient(string hubUrl, string identifier)
    {
        _hubUrl = hubUrl;
        Identifier = identifier;
        _connection = BuildConnection();
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        await _connection.StartAsync(cancellationToken);
    }

    // a new connection of the same device, like the cli after a restart
    public async Task ReconnectAsync(CancellationToken cancellationToken)
    {
        await _connection.DisposeAsync();
        _connection = BuildConnection();
        await _connection.StartAsync(cancellationToken);
    }

    private HubConnection BuildConnection()
    {
        var connection = new HubConnectionBuilder()
            .WithUrl(_hubUrl, options =>
            {
                options.Headers[Connections.IdentifierHeaderName] = Identifier;
            })
            .Build();

        connection.Register<IAppHubClient>(Events);
        Hub = connection.CreateHubProxy<IAppHub>();

        return connection;
    }

    public async ValueTask DisposeAsync()
    {
        await _connection.DisposeAsync();
    }
}
