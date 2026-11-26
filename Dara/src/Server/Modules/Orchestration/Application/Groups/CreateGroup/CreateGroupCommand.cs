using System.Text.Json.Serialization;
using Dara.Server.BuildingBlocks.Application.Commands;

namespace Dara.Server.Modules.Orchestration.Application.Groups.CreateGroup;

public record CreateGroupCommand(Guid GroupId, Guid OwnerId) : IOrderedInternalCommand
{
    // queued commands of one group run in order
    [JsonIgnore]
    public string OrderingKey => $"group:{GroupId}";
}
