using Dara.Server.BuildingBlocks.Application.Commands;

namespace Dara.Server.Modules.Clients.Application.Clients.EndClientSession;

public record EndClientSessionCommand(Guid ClientId) : ICommand;