using System.Text.Json.Serialization;
using Dara.Server.BuildingBlocks.Application.Commands;

namespace Dara.Server.Modules.Orchestration.Application.Groups.AddParticipantToGroup;

public record AddParticipantToGroupCommand(Guid GroupId, Guid ParticipantId) : IOrderedInternalCommand
{
    // queued commands of one group run in order
    [JsonIgnore]
    public string OrderingKey => $"group:{GroupId}";
}