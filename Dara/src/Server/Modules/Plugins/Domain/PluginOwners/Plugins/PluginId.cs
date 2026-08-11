using Dara.Server.BuildingBlocks.Domain;

namespace Dara.Server.Modules.Plugins.Domain.PluginOwners;

public record PluginId(Guid Value) : BaseEntityId(Value); 