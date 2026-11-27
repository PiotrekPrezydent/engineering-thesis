using System.Text.Json.Serialization;
using Dara.Server.BuildingBlocks.Application.Commands;

namespace Dara.Server.Modules.Orchestration.Application.Participants.UpdateParticipantContext;

public record UpdateParticipantContextCommand(Guid ParticipantId, IReadOnlyDictionary<string, string> Context) : IOrderedInternalCommand
{
    // queued commands of one participant run in order
    [JsonIgnore]
    public string OrderingKey => $"participant:{ParticipantId}";
}
