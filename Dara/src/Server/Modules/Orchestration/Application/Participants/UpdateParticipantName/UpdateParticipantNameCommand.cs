using System.Text.Json.Serialization;
using Dara.Server.BuildingBlocks.Application.Commands;

namespace Dara.Server.Modules.Orchestration.Application.Participants.UpdateParticipantName;

public record UpdateParticipantNameCommand(Guid ParticipantId, string NewName) : IOrderedInternalCommand
{
    // queued commands of one participant run in order
    [JsonIgnore]
    public string OrderingKey => $"participant:{ParticipantId}";
}