using System.Security.Cryptography;
using System.Text.Json;
using Dara.Clients.Core.Api;
using Dara.Shared.Common.CLI;
using Dara.Shared.Contracts.Plugins;

namespace Dara.Clients.Apps.CLI;

// commands and queries of the client over http (the client token of the session); the console passes only text, so
// commands with ids parse them. Groups, agent settings and the configuration of plugins are managed in the account
// panel, not here (TASK-ENG-032)
public class ApiCommands
{
    private static readonly JsonSerializerOptions ManifestJsonOptions = new(JsonSerializerDefaults.Web);

    private readonly AuthSession _auth;
    private readonly DaraApiClient _api;

    public ApiCommands(AuthSession auth, string serverUrl)
    {
        _auth = auth;
        _api = new DaraApiClient(new HttpClient { BaseAddress = new Uri(serverUrl) }, auth.GetAccessTokenAsync);
    }

    // a message with spaces in double quotes, a prompt starts with the prefix of the group, e.g. "/prompt take a photo"
    [CLICommand("sm")]
    public async Task SendGroupMessage(string groupId, string message)
    {
        await CallAsync(() => _api.SendGroupMessageAsync(Guid.Parse(groupId), message));
    }

    // a json file with one manifest or a list of them; the whole set replaces the registered one
    [CLICommand("rp")]
    public async Task RegisterPlugins(string manifestPath)
    {
        await CallAsync(async () =>
        {
            var plugins = ReadManifests(manifestPath);
            await _api.RegisterPluginsAsync(plugins);
            Console.WriteLine($"sent {plugins.Count} plugin(s), the server answers with OnPluginsRegistered when the set changed");
        });
    }

    [CLICommand("mp")]
    public async Task ShowMyPlugins()
    {
        await CallAsync(async () => HubEvents.PrintPlugins(await _api.GetCurrentClientPluginsAsync()));
    }

    [CLICommand("ar")]
    public async Task ShowAgentRun(string agentRunId)
    {
        await CallAsync(async () =>
        {
            var agentRun = await _api.GetAgentRunAsync(Guid.Parse(agentRunId));
            if (agentRun is null)
            {
                Console.WriteLine("no agent run: only the author of the prompt sees it");
                return;
            }

            Console.WriteLine($"agent run {agentRun.AgentRunId} in group {agentRun.GroupId}: {agentRun.Status} {agentRun.FailureReason}");
            HubEvents.PrintPlan(agentRun.AgentRunId, agentRun.Calls);
        });
    }

    [CLICommand("ap")]
    public async Task ApproveExecutionPlan(string agentRunId)
    {
        await CallAsync(() => _api.ApproveExecutionPlanAsync(Guid.Parse(agentRunId)));
    }

    [CLICommand("rj")]
    public async Task RejectExecutionPlan(string agentRunId)
    {
        await CallAsync(() => _api.RejectExecutionPlanAsync(Guid.Parse(agentRunId)));
    }

    // until the plugin host counts the hash of the .dll, a manifest without a hash gets the hash of its own json
    private static IReadOnlyList<PluginManifest> ReadManifests(string manifestPath)
    {
        var json = File.ReadAllText(manifestPath);
        var manifests = json.TrimStart().StartsWith('[')
            ? JsonSerializer.Deserialize<List<PluginManifest>>(json, ManifestJsonOptions)!
            : [JsonSerializer.Deserialize<PluginManifest>(json, ManifestJsonOptions)!];

        return manifests.Select(m => string.IsNullOrWhiteSpace(m.Sha256)
                ? m with { Sha256 = Convert.ToHexStringLower(SHA256.HashData(JsonSerializer.SerializeToUtf8Bytes(m, ManifestJsonOptions))) }
                : m)
            .ToList();
    }

    // a refused request shows its problem details, the console waits for the next command
    private async Task CallAsync(Func<Task> call)
    {
        if (!_auth.IsSignedIn)
        {
            Console.WriteLine("Sign in first: login <code> (the code comes from the account panel)");
            return;
        }

        try
        {
            await call();
        }
        catch (Exception e) when (e is DaraApiException or HttpRequestException or FormatException or IOException or JsonException)
        {
            Console.WriteLine($"error: {e.Message}");
        }
    }
}
