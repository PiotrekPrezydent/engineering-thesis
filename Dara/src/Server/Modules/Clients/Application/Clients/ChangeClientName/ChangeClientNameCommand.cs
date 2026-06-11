using Dara.Server.BuildingBlocks.Application.Commands;

namespace Dara.Server.Modules.Clients.Application.Clients.ChangeClientName;

public record ChangeClientNameCommand(Guid ClientId, string NewName) : ICommand;