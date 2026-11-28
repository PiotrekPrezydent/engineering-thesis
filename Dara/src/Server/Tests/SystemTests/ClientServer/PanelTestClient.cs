using System.Net.Http.Json;
using System.Text.Json;
using Dara.Shared.Contracts.Groups;
using Dara.Shared.Contracts.Orchestration;
using Dara.Shared.Contracts.Plugins;
using Dara.Shared.Contracts.StateSnapshots;

namespace Dara.Server.SystemTests.ClientServer;

// the account panel of one account (its session cookie), with the endpoints of groups, agent settings and plugins;
// the answers stay raw, so a test checks the status and the problem details itself
public class PanelTestClient
{
    private readonly AppHubTestHost _host;

    public string Cookie { get; }

    public PanelTestClient(AppHubTestHost host, string cookie)
    {
        _host = host;
        Cookie = cookie;
    }

    public Task<HttpResponseMessage> SendAsync(HttpMethod method, string path, object? body = null)
    {
        return _host.SendAsPanelAsync(method, path, Cookie, body);
    }

    public async Task<CreateGroupResponse> CreateGroupAsync(string name, Guid ownerClientId)
    {
        var response = await SendAsync(HttpMethod.Post, GroupRoutes.AccountGroups, new CreateGroupRequest(name, ownerClientId));
        await AppHubTestHost.EnsureAsync(response, $"group {name}");
        return (await response.Content.ReadFromJsonAsync<CreateGroupResponse>())!;
    }

    public Task<HttpResponseMessage> JoinGroupAsync(Guid clientId, Guid groupId, string joinCode)
    {
        return SendAsync(HttpMethod.Post, GroupRoutes.Join(clientId), new JoinGroupRequest(groupId, joinCode));
    }

    public Task<HttpResponseMessage> LeaveGroupAsync(Guid clientId, Guid groupId)
    {
        return SendAsync(HttpMethod.Delete, GroupRoutes.Membership(clientId, groupId));
    }

    public Task<HttpResponseMessage> GetAgentSettingsAsync(Guid groupId)
    {
        return SendAsync(HttpMethod.Get, AgentRunRoutes.AgentSettings(groupId));
    }

    public async Task<GroupAgentSettingsSnapshot> ReadAgentSettingsAsync(Guid groupId)
    {
        var response = await GetAgentSettingsAsync(groupId);
        await AppHubTestHost.EnsureAsync(response, $"agent settings of {groupId}");
        return (await response.Content.ReadFromJsonAsync<GroupAgentSettingsSnapshot>())!;
    }

    public Task<HttpResponseMessage> EditPromptPrefixAsync(Guid groupId, string prefix)
    {
        return SendAsync(HttpMethod.Put, AgentRunRoutes.PromptPrefix(groupId), new EditPromptMessagePrefixRequest(prefix));
    }

    public Task<HttpResponseMessage> EditProviderAsync(Guid groupId, string provider)
    {
        return SendAsync(HttpMethod.Put, AgentRunRoutes.Provider(groupId), new EditChatClientProviderRequest(provider));
    }

    public Task<HttpResponseMessage> EditModelAsync(Guid groupId, string model)
    {
        return SendAsync(HttpMethod.Put, AgentRunRoutes.Model(groupId), new EditClientModelRequest(model));
    }

    public Task<HttpResponseMessage> EditApiKeyAsync(Guid groupId, string apiKey)
    {
        return SendAsync(HttpMethod.Put, AgentRunRoutes.ApiKey(groupId), new EditApiKeyRequest(apiKey));
    }

    // the scripted provider of the test host with a model and a key
    public async Task SetModelAsync(Guid groupId, string provider, string model, string apiKey)
    {
        await AppHubTestHost.EnsureAsync(await EditProviderAsync(groupId, provider), "provider");
        await AppHubTestHost.EnsureAsync(await EditModelAsync(groupId, model), "model");
        await AppHubTestHost.EnsureAsync(await EditApiKeyAsync(groupId, apiKey), "api key");
    }

    public Task<HttpResponseMessage> GetPluginsAsync(Guid clientId)
    {
        return SendAsync(HttpMethod.Get, PluginRoutes.ClientPlugins(clientId));
    }

    public Task<HttpResponseMessage> EnablePluginAsync(Guid clientId, Guid pluginId)
    {
        return SendAsync(HttpMethod.Post, PluginRoutes.EnablePlugin(clientId, pluginId));
    }

    public Task<HttpResponseMessage> DisablePluginAsync(Guid clientId, Guid pluginId)
    {
        return SendAsync(HttpMethod.Post, PluginRoutes.DisablePlugin(clientId, pluginId));
    }

    public Task<HttpResponseMessage> EnableFunctionAsync(Guid clientId, Guid pluginId, Guid functionId)
    {
        return SendAsync(HttpMethod.Post, PluginRoutes.EnableFunction(clientId, pluginId, functionId));
    }

    public Task<HttpResponseMessage> DisableFunctionAsync(Guid clientId, Guid pluginId, Guid functionId)
    {
        return SendAsync(HttpMethod.Post, PluginRoutes.DisableFunction(clientId, pluginId, functionId));
    }

    // the "code", "rule" and "detail" of the problem details of a refused request
    public static async Task<(int Status, string? Code, string? Rule, string? Detail)> ProblemAsync(HttpResponseMessage response)
    {
        var body = await response.Content.ReadAsStringAsync();
        if (string.IsNullOrWhiteSpace(body))
            return ((int)response.StatusCode, null, null, null);

        using var problem = JsonDocument.Parse(body);
        return ((int)response.StatusCode, Text(problem.RootElement, "code"), Text(problem.RootElement, "rule"), Text(problem.RootElement, "detail"));
    }

    private static string? Text(JsonElement element, string name)
    {
        return element.TryGetProperty(name, out var value) && value.ValueKind == JsonValueKind.String ? value.GetString() : null;
    }
}
