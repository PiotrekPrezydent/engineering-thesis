using System.Text.Json.Serialization;
using Dara.Server.BuildingBlocks.Application.Commands;

namespace Dara.Server.Modules.Authorization.Application.Users.RemoveClientRoles;

// a client revoked in Identity loses its roles
public record RemoveClientRolesCommand(Guid UserId, Guid ClientId) : IOrderedInternalCommand
{
    // queued commands of one account run in order, so the account exists before its clients
    [JsonIgnore]
    public string OrderingKey => $"user:{UserId}";
}
