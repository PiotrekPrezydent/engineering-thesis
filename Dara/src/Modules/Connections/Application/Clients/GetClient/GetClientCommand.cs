using Dara.BuildingBlocks.Application.Commands;

namespace Dara.Modules.Connections.Application.Clients.GetClient
{
    public record GetClientCommand(string ConnectionId) : IModuleCommand;
}