using Dara.Server.BuildingBlocks.Application.Commands;

namespace Dara.Server.Modules.Identity.Application.ValidateAccountSession;

// the host checks the session of the panel cookie on every request; a command, because a valid use moves the idle
// lifetime on; the result is the user id of a valid session, null otherwise
public record ValidateAccountSessionCommand(Guid AccountSessionId) : ICommand<Guid?>;
