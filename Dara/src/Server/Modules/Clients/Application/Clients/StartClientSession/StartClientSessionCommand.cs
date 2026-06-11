using Dara.Server.BuildingBlocks.Application.Commands;

namespace Dara.Server.Modules.Clients.Application.Clients.StartClientSession;

public record StartClientSessionCommand(Guid ClientId, string ClientName) : ICommand;