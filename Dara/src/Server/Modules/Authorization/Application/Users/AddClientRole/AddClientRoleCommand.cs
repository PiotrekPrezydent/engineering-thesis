using Dara.Server.BuildingBlocks.Application.Commands;

namespace Dara.Server.Modules.Authorization.Application.Users.AddClientRole;

// the panel gives a client of the account a role; it counts when the client calls with its token
public record AddClientRoleCommand(Guid UserId, Guid ClientId, string RoleCode) : ICommand;
