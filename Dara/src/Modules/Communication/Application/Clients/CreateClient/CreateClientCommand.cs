using Dara.BuildingBlocks.Application.Abstraction;

namespace Dara.Modules.Communication.Application.Clients.CreateClient;

public record CreateClientCommand(string ConnectionId, string ConnectionIp) : IApplicationCommand;