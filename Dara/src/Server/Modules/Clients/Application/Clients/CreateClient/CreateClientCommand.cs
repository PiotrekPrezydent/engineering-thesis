using Dara.Server.BuildingBlocks.Application.Commands;

namespace Dara.Server.Modules.Clients.Application.Clients.CreateClient;

public record CreateClientCommand(Guid ClientId, string ClientName) : ICommand;