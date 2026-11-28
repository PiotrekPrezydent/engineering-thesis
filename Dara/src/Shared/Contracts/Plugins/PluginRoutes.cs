using Dara.Shared.Contracts.Auth;

namespace Dara.Shared.Contracts.Plugins;

// plugins of the client itself with the client token, plugins of a client of the account with the cookie of the
// account panel; TASK-ENG-031 replaces both with installations
public static class PluginRoutes
{
    public const string CurrentClientPlugins = "/clients/current/plugins";

    public static string ClientPlugins(Guid clientId) => $"{AuthRoutes.AccountClientsGroup}/{clientId}/plugins";
    public static string EnablePlugin(Guid clientId, Guid pluginId) => $"{ClientPlugins(clientId)}/{pluginId}/enable";
    public static string DisablePlugin(Guid clientId, Guid pluginId) => $"{ClientPlugins(clientId)}/{pluginId}/disable";
    public static string EnableFunction(Guid clientId, Guid pluginId, Guid functionId) => $"{ClientPlugins(clientId)}/{pluginId}/functions/{functionId}/enable";
    public static string DisableFunction(Guid clientId, Guid pluginId, Guid functionId) => $"{ClientPlugins(clientId)}/{pluginId}/functions/{functionId}/disable";
}
