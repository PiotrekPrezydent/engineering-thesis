using Dara.BuildingBlocks.Domain;

namespace Dara.Server.Modules.Plugins.Domain.PluginOwners;

public record PluginOwnerId(Guid Value) : BaseEntityId(Value);