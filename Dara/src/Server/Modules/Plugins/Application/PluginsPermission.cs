using Dara.Server.BuildingBlocks.Application.Authorization;

namespace Dara.Server.Modules.Plugins.Application;

// what a user may do in the Plugins module; commands and queries do not use it, endpoints of the API are marked with it
public sealed record PluginsPermission(PluginsPermission.Permissions Value) : ModulePermission<PluginsPermission.Permissions>(Value)
{
    public enum Permissions
    {
        ReadPlugins,
        RegisterPlugins,
        EnablePlugin,
        DisablePlugin,
        EnableFunction,
        DisableFunction
    }
}
