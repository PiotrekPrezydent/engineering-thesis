using Dara.Server.BuildingBlocks.Application.Commands;

namespace Dara.Server.Modules.Clients.Application.Clients.DeleteClient;

public record DeleteClientCommand(Guid ClientId) : ICommand;