using System.Text.Json.Serialization;
using Dara.Server.BuildingBlocks.Application.Commands;

namespace Dara.Server.Modules.Groups.Application.Members.CreateMember;

// a client registered in identity; the member id is the id of the client
public record CreateMemberCommand(
    Guid MemberId,
    Guid UserId,
    string Name) : IOrderedInternalCommand
{
    // queued commands of one member run in order
    [JsonIgnore]
    public string OrderingKey => $"member:{MemberId}";
}
