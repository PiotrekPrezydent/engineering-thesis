using Dara.Server.BuildingBlocks.Application.Commands;

namespace Dara.Server.Modules.Identity.Application.RevokeClient;

// final; the panel of the account
public record RevokeClientCommand(Guid UserId, Guid ClientId) : ICommand;
