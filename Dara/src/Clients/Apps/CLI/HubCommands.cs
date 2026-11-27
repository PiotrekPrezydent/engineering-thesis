using System.Security.Cryptography;
using System.Text.Json;
using Dara.Shared.Common.CLI;
using Dara.Shared.Contracts;
using Dara.Shared.Contracts.Plugins;
using Dara.Shared.Contracts.StateSnapshots;
using Microsoft.AspNetCore.SignalR;

namespace Dara.Clients.Apps.CLI;

// the console passes only text, so commands with ids parse them and call the typed hub methods
public class HubCommands : IAppHub
{
    private static readonly JsonSerializerOptions ManifestJsonOptions = new(JsonSerializerDefaults.Web);

    public static IAppHub Proxy { get; set; }

    public HubCommands()
    {
    }

    [CLICommand("cg")]
    public async Task CreateGroup(string groupName)
    {
        await CallAsync(() => Proxy.CreateGroup(groupName));
    }

    [CLICommand("jg")]
    public async Task JoinGroup(string groupId, string joinCode)
    {
        await CallAsync(() => JoinGroup(Guid.Parse(groupId), joinCode));
    }

    public async Task JoinGroup(Guid groupId, string joinCode)
    {
        await Proxy.JoinGroup(groupId, joinCode);
    }

    [CLICommand("lg")]
    public async Task LeaveGroup(string groupId)
    {
        await CallAsync(() => LeaveGroup(Guid.Parse(groupId)));
    }

    public async Task LeaveGroup(Guid groupId)
    {
        await Proxy.LeaveGroup(groupId);
    }

    // a message with spaces in double quotes, a prompt starts with the prefix of the group, e.g. "/prompt take a photo"
    [CLICommand("sm")]
    public async Task SendGroupMessage(string groupId, string message)
    {
        await CallAsync(() => SendGroupMessage(Guid.Parse(groupId), message));
    }

    public async Task SendGroupMessage(Guid groupId, string message)
    {
        await Proxy.SendGroupMessage(groupId, message);
    }

    // a json file with one manifest or a list of them; the whole set replaces the registered one
    [CLICommand("rp")]
    public async Task RegisterPlugins(string manifestPath)
    {
        await CallAsync(async () =>
        {
            var plugins = ReadManifests(manifestPath);
            await RegisterPlugins(plugins);
            Console.WriteLine($"sent {plugins.Count} plugin(s), the server answers with OnPluginsRegistered when the set changed");
        });
    }

    public async Task RegisterPlugins(IReadOnlyList<PluginManifest> plugins)
    {
        await Proxy.RegisterPlugins(plugins);
    }

    [CLICommand("mp")]
    public async Task ShowMyPlugins()
    {
        await CallAsync(async () => HubEvents.PrintPlugins(await GetMyPlugins()));
    }

    public async Task<IReadOnlyList<PluginSnapshot>> GetMyPlugins()
    {
        return await Proxy.GetMyPlugins();
    }

    [CLICommand("ep")]
    public async Task EnablePlugin(string pluginId)
    {
        await CallAsync(() => EnablePlugin(Guid.Parse(pluginId)));
    }

    public async Task EnablePlugin(Guid pluginId)
    {
        await Proxy.EnablePlugin(pluginId);
    }

    [CLICommand("dp")]
    public async Task DisablePlugin(string pluginId)
    {
        await CallAsync(() => DisablePlugin(Guid.Parse(pluginId)));
    }

    public async Task DisablePlugin(Guid pluginId)
    {
        await Proxy.DisablePlugin(pluginId);
    }

    [CLICommand("ef")]
    public async Task EnablePluginFunction(string pluginId, string functionId)
    {
        await CallAsync(() => EnablePluginFunction(Guid.Parse(pluginId), Guid.Parse(functionId)));
    }

    public async Task EnablePluginFunction(Guid pluginId, Guid functionId)
    {
        await Proxy.EnablePluginFunction(pluginId, functionId);
    }

    [CLICommand("df")]
    public async Task DisablePluginFunction(string pluginId, string functionId)
    {
        await CallAsync(() => DisablePluginFunction(Guid.Parse(pluginId), Guid.Parse(functionId)));
    }

    public async Task DisablePluginFunction(Guid pluginId, Guid functionId)
    {
        await Proxy.DisablePluginFunction(pluginId, functionId);
    }

    [CLICommand("gs")]
    public async Task ShowGroupAgentSettings(string groupId)
    {
        await CallAsync(async () =>
        {
            var settings = await GetGroupAgentSettings(Guid.Parse(groupId));
            Console.WriteLine(settings?.ToString() ?? "no settings: not a participant of the group");
        });
    }

    public async Task<GroupAgentSettingsSnapshot?> GetGroupAgentSettings(Guid groupId)
    {
        return await Proxy.GetGroupAgentSettings(groupId);
    }

    [CLICommand("gpre")]
    public async Task EditGroupPromptMessagePrefix(string groupId, string promptMessagePrefix)
    {
        await CallAsync(() => EditGroupPromptMessagePrefix(Guid.Parse(groupId), promptMessagePrefix));
    }

    public async Task EditGroupPromptMessagePrefix(Guid groupId, string promptMessagePrefix)
    {
        await Proxy.EditGroupPromptMessagePrefix(groupId, promptMessagePrefix);
    }

    [CLICommand("gprov")]
    public async Task EditGroupChatClientProviderName(string groupId, string chatClientProviderName)
    {
        await CallAsync(() => EditGroupChatClientProviderName(Guid.Parse(groupId), chatClientProviderName));
    }

    public async Task EditGroupChatClientProviderName(Guid groupId, string chatClientProviderName)
    {
        await Proxy.EditGroupChatClientProviderName(groupId, chatClientProviderName);
    }

    [CLICommand("gmod")]
    public async Task EditGroupClientModelName(string groupId, string clientModelName)
    {
        await CallAsync(() => EditGroupClientModelName(Guid.Parse(groupId), clientModelName));
    }

    public async Task EditGroupClientModelName(Guid groupId, string clientModelName)
    {
        await Proxy.EditGroupClientModelName(groupId, clientModelName);
    }

    [CLICommand("gkey")]
    public async Task EditGroupApiKey(string groupId, string apiKey)
    {
        await CallAsync(() => EditGroupApiKey(Guid.Parse(groupId), apiKey));
    }

    public async Task EditGroupApiKey(Guid groupId, string apiKey)
    {
        await Proxy.EditGroupApiKey(groupId, apiKey);
    }

    [CLICommand("ar")]
    public async Task ShowAgentRun(string agentRunId)
    {
        await CallAsync(async () =>
        {
            var agentRun = await GetAgentRun(Guid.Parse(agentRunId));
            if (agentRun is null)
            {
                Console.WriteLine("no agent run: only the author of the prompt sees it");
                return;
            }

            Console.WriteLine($"agent run {agentRun.AgentRunId} in group {agentRun.GroupId}: {agentRun.Status} {agentRun.FailureReason}");
            HubEvents.PrintPlan(agentRun.AgentRunId, agentRun.Calls);
        });
    }

    public async Task<AgentRunSnapshot?> GetAgentRun(Guid agentRunId)
    {
        return await Proxy.GetAgentRun(agentRunId);
    }

    [CLICommand("ap")]
    public async Task ApproveExecutionPlan(string agentRunId)
    {
        await CallAsync(() => ApproveExecutionPlan(Guid.Parse(agentRunId)));
    }

    public async Task ApproveExecutionPlan(Guid agentRunId)
    {
        await Proxy.ApproveExecutionPlan(agentRunId);
    }

    [CLICommand("rj")]
    public async Task RejectExecutionPlan(string agentRunId)
    {
        await CallAsync(() => RejectExecutionPlan(Guid.Parse(agentRunId)));
    }

    public async Task RejectExecutionPlan(Guid agentRunId)
    {
        await Proxy.RejectExecutionPlan(agentRunId);
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

    // a broken rule comes back as a hub exception, the console shows it and waits for the next command
    private static async Task CallAsync(Func<Task> call)
    {
        if (Proxy is null)
        {
            Console.WriteLine("not connected: con or con-u <identifier> first");
            return;
        }

        try
        {
            await call();
        }
        catch (Exception e) when (e is HubException or FormatException or IOException or JsonException)
        {
            Console.WriteLine($"error: {e.Message}");
        }
    }
}
