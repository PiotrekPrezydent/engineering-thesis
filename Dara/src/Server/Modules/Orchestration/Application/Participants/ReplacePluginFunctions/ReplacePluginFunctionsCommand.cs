using System.Text.Json.Serialization;
using Dara.Server.BuildingBlocks.Application.Commands;
using Dara.Server.Modules.Plugins.Integration;

namespace Dara.Server.Modules.Orchestration.Application.Participants.ReplacePluginFunctions;

public record ReplacePluginFunctionsCommand(Guid ParticipantId, Guid PluginId, int Version, IReadOnlyList<AvailableFunctionSnapshot> Functions) : IOrderedInternalCommand
{
    // queued commands of one participant run in order
    [JsonIgnore]
    public string OrderingKey => $"participant:{ParticipantId}";
}
