using Dara.Server.BuildingBlocks.Application;

namespace Dara.Server.Modules.Identity.Application;

public interface IIdentityModule : IModule
{
    // the host checks the session of the panel cookie on every request; not a command, because the command bus takes
    // only business commands on known ids; a valid use moves the idle lifetime on and is saved here; the user id of a
    // valid session, null otherwise
    Task<Guid?> AuthenticateAccountSessionAsync(Guid accountSessionId);
}
