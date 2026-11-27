using Dara.Server.BuildingBlocks.Application.Commands;

namespace Dara.Server.Modules.Identity.Application.SignOut;

// the host takes both ids from the claims of the client token
public record SignOutClientCommand(Guid UserId, Guid ClientId) : ICommand;
