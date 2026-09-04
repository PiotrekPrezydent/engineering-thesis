using Dara.Server.BuildingBlocks.Domain;

namespace Dara.Server.Modules.Plugins.Domain.Plugins;

public record PluginFunctionId(Guid Value) : BaseEntityId(Value);
