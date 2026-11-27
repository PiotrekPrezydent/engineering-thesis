using Dara.Server.BuildingBlocks.Application.Commands;

namespace Dara.Server.Modules.Authorization.Application.Users.RemoveAccountRole;

// the panel takes a role from the account
public record RemoveAccountRoleCommand(Guid UserId, string RoleCode) : ICommand;
