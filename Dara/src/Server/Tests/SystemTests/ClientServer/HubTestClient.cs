using System.Net.Http.Json;
using Dara.Clients.Core.Api;
using Dara.Shared.Contracts;
using Dara.Shared.Contracts.Auth;
using Microsoft.AspNetCore.SignalR.Client;
using TypedSignalR.Client;

namespace Dara.Server.SystemTests.ClientServer;

// one signed-in client built like the cli: commands and queries over http (Api), notifications over the hub with
// a typed receiver; a client token from its session, so a new connection is the same client
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
    public DaraApiClient Api { get; }
    public HubConnection Connection => _connection;

    // kept across connections, so a test sees everything the client got
    public RecordingHubClient Events { get; } = new();

    // a silent client sends no keep-alive for an hour and waits as long for the server, so the server takes it for a
    // broken connection after its ClientTimeoutInterval
    private readonly TimeSpan _keepAlive;
    private readonly TimeSpan _serverTimeout;

    public HubTestClient(string hubUrl, HttpClient http, string identifier, AuthTokensResponse tokens, TimeSpan keepAlive, TimeSpan serverTimeout,
        bool silent = false)
    {
        _keepAlive = silent ? TimeSpan.FromHours(1) : keepAlive;
        _serverTimeout = silent ? TimeSpan.FromHours(2) : serverTimeout;
        _hubUrl = hubUrl;
        _http = http;
        Identifier = identifier;
        ClientId = tokens.ClientId;
        Tokens = tokens;
        UserId = tokens.UserId;
        Api = new DaraApiClient(http, GetAccessTokenAsync);
        _connection = BuildConnection();
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        await _connection.StartAsync(cancellationToken);
    }

    // the renewal of a token (TASK-ENG-032-D08): the new connection opens before the old one closes, so the client
    // never looks offline
    public async Task RenewAsync(CancellationToken cancellationToken)
    {
        var old = _connection;
        var renewed = BuildConnection();
        await renewed.StartAsync(cancellationToken);
        _connection = renewed;
        await old.DisposeAsync();
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
            .WithKeepAliveInterval(_keepAlive)
            .WithServerTimeout(_serverTimeout)
            .Build();

        connection.Register<IAppHubClient>(Events);

        return connection;
    }

    public async ValueTask DisposeAsync()
    {
        await _connection.DisposeAsync();
    }
}
