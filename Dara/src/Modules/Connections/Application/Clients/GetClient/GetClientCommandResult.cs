using Dara.BuildingBlocks.Application.Commands;

namespace Dara.Modules.Connections.Application.Clients.GetClient
{
    public record GetClientCommandResult(Guid ClientId) : IModuleCommandResult;
}