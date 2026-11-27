using System.Text.Json.Serialization;
using Dara.Server.BuildingBlocks.Application.Commands;

namespace Dara.Server.Modules.Orchestration.Application.Participants.CreateParticipant;

// a client registered in identity; the participant id is the id of the client
public record CreateParticipantCommand(Guid ParticipantId, string Name) : IOrderedInternalCommand
{
    // queued commands of one participant run in order
    [JsonIgnore]
    public string OrderingKey => $"participant:{ParticipantId}";
}
