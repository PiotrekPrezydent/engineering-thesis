using Dara.Server.BuildingBlocks.Application.Commands;

namespace Dara.Server.Modules.Identity.Application.SignOut;

// the host takes both ids from the claims of the panel cookie ("sub" and "sid")
public record SignOutAccountCommand(Guid UserId, Guid PanelSignInId) : ICommand;
