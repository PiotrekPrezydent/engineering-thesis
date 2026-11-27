using Dara.Server.BuildingBlocks.Application;
using Dara.Server.Modules.Identity.Application.AuthenticatePanelSignIn;

namespace Dara.Server.Modules.Identity.Application;

public interface IIdentityModule : IModule
{
    // the host checks the sign-in of the panel cookie on every request; not a command, because the command bus takes
    // only business commands on known ids; a valid use moves the idle lifetime on and is saved here; the account and
    // its panel client of a valid sign-in, null otherwise
    Task<PanelSignInResult?> AuthenticatePanelSignInAsync(Guid panelSignInId);
}
