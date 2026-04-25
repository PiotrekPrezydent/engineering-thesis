using Dara.BuildingBlocks.Application.Commands;

namespace Dara.Modules.Connections.Application.Clients.CreateClient
{
    public record CreateClientCommandResult(Guid ClientId) : IModuleCommandResult;
}