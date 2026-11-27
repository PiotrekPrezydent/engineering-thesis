using Dara.Server.BuildingBlocks.Application.Commands;

namespace Dara.Server.Modules.Identity.Application.CreateClientRegistrationCode;

// the account panel asks for a code for a new client; the host takes UserId from the panel cookie
public record CreateClientRegistrationCodeCommand(Guid UserId) : ICommand<ClientCodeResult>;
