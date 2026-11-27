using System.Net.Http.Json;
using System.Text.Json;
using Dara.Shared.Common.CLI;
using Dara.Shared.Contracts.Auth;

namespace Dara.Clients.Apps.CLI;

// sign-in of the cli as a client (minimal, the full client is TASK-ENG-034): a new cli registers itself with a
// registration code of the account panel, an existing one signs in with a login code; it keeps the tokens and the
// client id only in memory (a session that outlives the program is TASK-ENG-034)
public class AuthSession
{
    // a token that expires sooner is refreshed before it is given to the connection
    private static readonly TimeSpan RefreshMargin = TimeSpan.FromSeconds(30);

    private readonly HttpClient _http;
    // one refresh at a time: two refreshes with one token would end the session (refresh_reused)
    private readonly SemaphoreSlim _refreshLock = new(1, 1);
    private AuthTokensResponse? _tokens;

    public AuthSession(string serverUrl)
    {
        _http = new HttpClient { BaseAddress = new Uri(serverUrl) };
    }

    public bool IsSignedIn => _tokens != null;

    // used by the hub connection on every (re)connect
    public async Task<string?> GetAccessTokenAsync()
    {
        await _refreshLock.WaitAsync();
        try
        {
            if (_tokens == null)
                return null;

            if (_tokens.AccessTokenExpiresAt - DateTime.UtcNow < RefreshMargin)
                await RefreshCoreAsync();

            return _tokens?.AccessToken;
        }
        finally
        {
            _refreshLock.Release();
        }
    }

    // the registration code of the account panel makes this cli a new client of the account, type Pc with the
    // default name ("PC 2"); the panel renames it
    [CLICommand("register")]
    async Task Register(string code)
    {
        var request = new RegisterClientRequest(code, "Pc", null);
        await SignInAsync("Registration", await _http.PostAsJsonAsync(AuthRoutes.RegisterClient, request));
    }

    // the login code of the account panel, for a client registered before (a copy of the cli on another computer)
    [CLICommand("login")]
    async Task Login(string code)
    {
        await SignInAsync("Sign-in", await _http.PostAsJsonAsync(AuthRoutes.ClientLogin, new ClientLoginRequest(code)));
    }

    private async Task SignInAsync(string what, HttpResponseMessage response)
    {
        if (!response.IsSuccessStatusCode)
        {
            await PrintRefusalAsync(what, response);
            return;
        }

        _tokens = await response.Content.ReadFromJsonAsync<AuthTokensResponse>();
        Console.WriteLine($"Signed in, user id {_tokens!.UserId}, client id {_tokens.ClientId}");
    }

    [CLICommand("logout")]
    async Task Logout()
    {
        var accessToken = await GetAccessTokenAsync();
        if (accessToken == null)
        {
            Console.WriteLine("Not signed in");
            return;
        }

        using var request = new HttpRequestMessage(HttpMethod.Post, AuthRoutes.Logout);
        request.Headers.Authorization = new("Bearer", accessToken);
        var response = await _http.SendAsync(request);
        _tokens = null;

        Console.WriteLine(response.IsSuccessStatusCode ? "Signed out" : $"Sign-out answered {(int)response.StatusCode}, tokens dropped");
    }

    private async Task RefreshCoreAsync()
    {
        var response = await _http.PostAsJsonAsync(AuthRoutes.Refresh, new RefreshRequest(_tokens!.RefreshToken));
        if (!response.IsSuccessStatusCode)
        {
            // every refusal of a refresh means: sign in again with a new code
            _tokens = null;
            await PrintRefusalAsync("Refresh", response);
            Console.WriteLine("Get a new code in the account panel and sign in with: login <code>");
            return;
        }

        _tokens = await response.Content.ReadFromJsonAsync<AuthTokensResponse>();
    }

    // prints the problem details of a refusal
    private static async Task PrintRefusalAsync(string what, HttpResponseMessage response)
    {
        var body = await response.Content.ReadAsStringAsync();
        string? code = null;
        string? detail = null;
        try
        {
            using var problem = JsonDocument.Parse(body);
            if (problem.RootElement.TryGetProperty("code", out var codeElement))
                code = codeElement.GetString();
            if (problem.RootElement.TryGetProperty("detail", out var detailElement))
                detail = detailElement.GetString();
        }
        catch (JsonException)
        {
        }

        Console.WriteLine($"{what} refused: {(int)response.StatusCode} {code} {detail ?? body}");
    }
}
