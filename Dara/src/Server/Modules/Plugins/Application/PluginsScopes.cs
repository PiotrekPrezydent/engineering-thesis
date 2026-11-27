using Dara.Server.BuildingBlocks.Application.Authorization;

namespace Dara.Server.Modules.Plugins.Application;

// the kinds of resources of the module whose access is controlled: one plugin (ScopeId = the plugin id) and one
// function of a plugin (ScopeId = the function id), a child scope of the plugin
public static class PluginsScopes
{
    public static readonly ScopeDefinition Plugin = new("Plugins.Plugin");
    public static readonly ScopeDefinition Function = new("Plugins.Plugin.Function");
}
