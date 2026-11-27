using Dara.Server.BuildingBlocks.Application.Commands;

namespace Dara.Server.Modules.Identity.Application.IssueClientLoginCode;

// the account panel asks for a code for one of its clients (a copy of the app on another computer); the host takes
// UserId from the account session
public record IssueClientLoginCodeCommand(Guid UserId, Guid ClientId) : ICommand<ClientCodeResult>;
