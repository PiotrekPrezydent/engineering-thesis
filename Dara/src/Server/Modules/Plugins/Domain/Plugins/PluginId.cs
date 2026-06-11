using Dara.Server.BuildingBlocks.Domain;

namespace Dara.Server.Modules.Plugins.Domain.Plugins;

public record PluginId(Guid Value) : BaseEntityId(Value);