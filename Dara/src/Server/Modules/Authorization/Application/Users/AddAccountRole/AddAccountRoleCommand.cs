using Dara.Server.BuildingBlocks.Application.Commands;

namespace Dara.Server.Modules.Authorization.Application.Users.AddAccountRole;

// the panel gives the account a role; it counts for the panel (cookie)
public record AddAccountRoleCommand(Guid UserId, string RoleCode) : ICommand;
