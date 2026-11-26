using System.Text.Json.Serialization;
using Dara.Server.BuildingBlocks.Application.Commands;

namespace Dara.Server.Modules.Groups.Application.Members.CreateMember;

public record CreateDefaultMemberCommand(Guid MemberId) : IOrderedInternalCommand
{
    // queued commands of one member run in order
    [JsonIgnore]
    public string OrderingKey => $"member:{MemberId}";
}