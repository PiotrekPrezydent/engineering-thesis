using Dara.Server.BuildingBlocks.Application.Commands;

namespace Dara.Server.Modules.Authorization.Application.Users.RemoveClientRole;

// the panel takes a role from a client of the account
public record RemoveClientRoleCommand(Guid UserId, Guid ClientId, string RoleCode) : ICommand;
