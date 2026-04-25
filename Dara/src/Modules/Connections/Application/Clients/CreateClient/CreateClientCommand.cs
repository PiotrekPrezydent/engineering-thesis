using Dara.BuildingBlocks.Application.Commands;

namespace Dara.Modules.Connections.Application.Clients.CreateClient
{
    public record CreateClientCommand(string ConnectionId, string ClientName, string ClientAuthToken) : IModuleCommand;
}