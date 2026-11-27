using System.Text.Json.Serialization;
using Dara.Server.BuildingBlocks.Application.Commands;

namespace Dara.Server.Modules.Authorization.Application.Users.CreateUserRoles;

// a new account in Identity gets the default roles
public record CreateUserRolesCommand(Guid UserId) : IOrderedInternalCommand
{
    // queued commands of one account run in order, so the account exists before its clients
    [JsonIgnore]
    public string OrderingKey => $"user:{UserId}";
}
