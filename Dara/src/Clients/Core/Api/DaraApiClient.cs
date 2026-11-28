using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using Dara.Shared.Contracts.Clients;
using Dara.Shared.Contracts.Groups;
using Dara.Shared.Contracts.Orchestration;
using Dara.Shared.Contracts.Plugins;
using Dara.Shared.Contracts.StateSnapshots;

namespace Dara.Clients.Core.Api;

// the http api of a client (client token): every command and query of the client, one method per endpoint; the hub
// only brings notifications. The token provider refreshes the token when it expires (the session of the app)
public class DaraApiClient
{
    private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web);

    private readonly HttpClient _http;
    private readonly Func<Task<string?>> _accessTokenProvider;

    // the http client has the base address of the server
    public DaraApiClient(HttpClient http, Func<Task<string?>> accessTokenProvider)
    {
        _http = http;
        _accessTokenProvider = accessTokenProvider;
    }

    public Task SendGroupMessageAsync(Guid groupId, string content, CancellationToken cancellationToken = default)
    {
        return SendAsync(HttpMethod.Post, GroupRoutes.Messages(groupId), new SendGroupMessageRequest(content), cancellationToken);
    }

    // null for a missing run and for a run of another author
    public Task<AgentRunSnapshot?> GetAgentRunAsync(Guid agentRunId, CancellationToken cancellationToken = default)
    {
        return GetOrNullAsync<AgentRunSnapshot>(AgentRunRoutes.AgentRun(agentRunId), cancellationToken);
    }

    public Task ApproveExecutionPlanAsync(Guid agentRunId, CancellationToken cancellationToken = default)
    {
        return SendAsync(HttpMethod.Post, AgentRunRoutes.Approve(agentRunId), null, cancellationToken);
    }

    public Task RejectExecutionPlanAsync(Guid agentRunId, CancellationToken cancellationToken = default)
    {
        return SendAsync(HttpMethod.Post, AgentRunRoutes.Reject(agentRunId), null, cancellationToken);
    }

    public async Task<AccountClientSnapshot> GetCurrentClientAsync(CancellationToken cancellationToken = default)
    {
        return (await GetOrNullAsync<AccountClientSnapshot>(ClientRoutes.Current, cancellationToken))!;
    }

    public async Task<IReadOnlyList<PluginSnapshot>> GetCurrentClientPluginsAsync(CancellationToken cancellationToken = default)
    {
        return (await GetOrNullAsync<List<PluginSnapshot>>(PluginRoutes.CurrentClientPlugins, cancellationToken))!;
    }

    // the whole set replaces the registered one (until TASK-ENG-031)
    public Task RegisterPluginsAsync(IReadOnlyList<PluginManifest> plugins, CancellationToken cancellationToken = default)
    {
        return SendAsync(HttpMethod.Put, PluginRoutes.CurrentClientPlugins, new RegisterPluginsRequest(plugins), cancellationToken);
    }

    protected async Task SendAsync(HttpMethod method, string path, object? body, CancellationToken cancellationToken)
    {
        using var response = await SendRawAsync(method, path, body, cancellationToken);
        await EnsureAsync(response, cancellationToken);
    }

    private async Task<T?> GetOrNullAsync<T>(string path, CancellationToken cancellationToken)
    {
        using var response = await SendRawAsync(HttpMethod.Get, path, null, cancellationToken);
        if (response.StatusCode == HttpStatusCode.NotFound)
            return default;

        await EnsureAsync(response, cancellationToken);
        return await response.Content.ReadFromJsonAsync<T>(JsonOptions, cancellationToken);
    }

    private async Task<HttpResponseMessage> SendRawAsync(HttpMethod method, string path, object? body, CancellationToken cancellationToken)
    {
        using var request = new HttpRequestMessage(method, path);
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", await _accessTokenProvider());
        if (body != null)
            request.Content = JsonContent.Create(body, body.GetType(), options: JsonOptions);

        return await _http.SendAsync(request, cancellationToken);
    }

    private static async Task EnsureAsync(HttpResponseMessage response, CancellationToken cancellationToken)
    {
        if (response.IsSuccessStatusCode)
            return;

        string? code = null, rule = null, detail = null;
        if (response.Content.Headers.ContentLength is not 0)
        {
            try
            {
                using var problem = JsonDocument.Parse(await response.Content.ReadAsStringAsync(cancellationToken));
                code = Text(problem.RootElement, "code");
                rule = Text(problem.RootElement, "rule");
                detail = Text(problem.RootElement, "detail");
            }
            catch (JsonException)
            {
                // no problem details, the status says enough
            }
        }

        throw new DaraApiException(response.StatusCode, code, rule, detail);
    }

    private static string? Text(JsonElement element, string name)
    {
        return element.ValueKind == JsonValueKind.Object && element.TryGetProperty(name, out var value) && value.ValueKind == JsonValueKind.String
            ? value.GetString()
            : null;
    }
}
