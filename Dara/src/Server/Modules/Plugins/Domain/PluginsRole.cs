using Dara.Server.BuildingBlocks.Domain.Roles;

namespace Dara.Server.Modules.Plugins.Domain;

public sealed record PluginsRole(PluginsRole.Roles Value) : EntityRole<PluginsRole.Roles>(Value)
{
    public enum Roles
    {
        Owner,
        Viewer
    }
}
