using Dara.BuildingBlocks.Application;

namespace Dara.Modules.Connections.Application.Clients.CreateClient
{
    public record CreateClientCommand(string ConnectionId, string ClientName, string ClientAuthToken) : IModuleCommand;
}