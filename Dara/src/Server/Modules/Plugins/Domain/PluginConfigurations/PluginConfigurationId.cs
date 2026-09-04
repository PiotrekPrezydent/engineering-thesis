using Dara.Server.BuildingBlocks.Domain;

namespace Dara.Server.Modules.Plugins.Domain.PluginConfigurations;

public record PluginConfigurationId(Guid Value) : BaseEntityId(Value);