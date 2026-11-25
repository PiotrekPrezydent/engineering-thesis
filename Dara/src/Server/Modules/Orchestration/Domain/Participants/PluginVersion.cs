using Dara.Server.BuildingBlocks.Domain;
using Dara.Server.Modules.Orchestration.Domain.Participants.Functions;

namespace Dara.Server.Modules.Orchestration.Domain.Participants;

// version of the last applied state of one plugin's functions
public record PluginVersion(PluginId PluginId, int Version) : IValueObject;
