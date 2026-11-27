using Dara.Server.BuildingBlocks.Application.Commands;

namespace Dara.Server.Modules.Identity.Application.RenameClient;

// the panel only: in the app of the client the name is read-only
public record RenameClientCommand(Guid UserId, Guid ClientId, string Name) : ICommand;
