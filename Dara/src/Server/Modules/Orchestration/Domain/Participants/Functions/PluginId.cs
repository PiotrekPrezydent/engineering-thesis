using Dara.Server.BuildingBlocks.Domain;

namespace Dara.Server.Modules.Orchestration.Domain.Participants.Functions;

// plugin of the plugins module the functions come from
public record PluginId(Guid Value) : BaseEntityId(Value);
