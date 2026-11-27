using System.Net.Http.Json;
using Dara.Shared.Contracts;
using Dara.Shared.Contracts.Auth;
using Microsoft.AspNetCore.SignalR.Client;
using TypedSignalR.Client;

namespace Dara.Server.SystemTests.ClientServer;

// one signed-in client built like DaraConnection of the cli: a client token from AccessTokenProvider,
// typed hub proxy and typed receiver; it keeps its tokens, so a new connection is the same client
public class HubTestClient : IAsyncDisposable
{
    // like the cli: a token that expires sooner is refreshed before a connection
    private static readonly TimeSpan RefreshMargin = TimeSpan.FromSeconds(1);

    private readonly string _hubUrl;
    private readonly HttpClient _http;
    // one refresh at a time, two refreshes with one token would end the session
    private readonly SemaphoreSlim _refreshLock = new(1, 1);
    private HubConnection _connection;

    public string Identifier { get; }
    public Guid UserId { get; set; }
    public Guid ClientId { get; }
    public AuthTokensResponse Tokens { get; private set; }
    public IAppHub Hub { get; private set; } = null!;
    public HubConnection Connection => _connection;

    // kept across connections, so a test sees everything the client got
    public RecordingHubClient Events { get; } = new();

    public HubTestClient(string hubUrl, HttpClient http, string identifier, AuthTokensResponse tokens)
    {
        _hubUrl = hubUrl;
        _http = http;
        Identifier = identifier;
        ClientId = tokens.ClientId;
        Tokens = tokens;
        UserId = tokens.UserId;
        _connection = BuildConnection();
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        await _connection.StartAsync(cancellationToken);
    }

    // a new connection of the same client, like the cli after a restart
    public async Task ReconnectAsync(CancellationToken cancellationToken)
    {
        await _connection.DisposeAsync();
        _connection = BuildConnection();
        await _connection.StartAsync(cancellationToken);
    }

    public async Task<string?> GetAccessTokenAsync()
    {
        await _refreshLock.WaitAsync();
        try
        {
            if (Tokens.AccessTokenExpiresAt - DateTime.UtcNow < RefreshMargin)
                await RefreshCoreAsync();

            return Tokens.AccessToken;
        }
        finally
        {
            _refreshLock.Release();
        }
    }

    private async Task RefreshCoreAsync()
    {
        var response = await _http.PostAsJsonAsync(AuthRoutes.Refresh, new RefreshRequest(Tokens.RefreshToken));
        response.EnsureSuccessStatusCode();
        Tokens = (await response.Content.ReadFromJsonAsync<AuthTokensResponse>())!;
    }

    private HubConnection BuildConnection()
    {
        var connection = new HubConnectionBuilder()
            .WithUrl(_hubUrl, options =>
            {
                options.AccessTokenProvider = GetAccessTokenAsync;
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
