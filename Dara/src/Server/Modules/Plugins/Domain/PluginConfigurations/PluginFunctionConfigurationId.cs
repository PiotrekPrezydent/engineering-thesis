using Dara.Server.BuildingBlocks.Domain;

namespace Dara.Server.Modules.Plugins.Domain.PluginConfigurations;

public record PluginFunctionConfigurationId(Guid Value) : BaseEntityId(Value);