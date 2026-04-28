using Dara.BuildingBlocks.Application;

namespace Dara.Modules.Connections.Application.Clients.GetClient
{
    public record GetClientCommand(string ConnectionId) : IModuleCommand;
}