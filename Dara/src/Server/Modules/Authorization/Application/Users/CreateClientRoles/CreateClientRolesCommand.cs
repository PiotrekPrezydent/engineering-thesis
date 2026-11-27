using System.Text.Json.Serialization;
using Dara.Server.BuildingBlocks.Application.Commands;

namespace Dara.Server.Modules.Authorization.Application.Users.CreateClientRoles;

// a new client of an account in Identity gets the default roles
public record CreateClientRolesCommand(Guid UserId, Guid ClientId) : IOrderedInternalCommand
{
    // queued commands of one account run in order, so the account exists before its clients
    [JsonIgnore]
    public string OrderingKey => $"user:{UserId}";
}
