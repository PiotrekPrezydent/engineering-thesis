using Dara.BuildingBlocks.Application.Abstraction;

namespace Dara.Modules.Connections.Application.Clients.CreateClient
{
    public record CreateClientCommand(string ConnectionId, string ClientName, string ClientAuthToken) : IApplicationCommand;
}