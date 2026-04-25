using Dara.BuildingBlocks.Application.Abstraction;

namespace Dara.Modules.Connections.Application.Clients.CreateClient
{
    public record CreateClientCommandResult(Guid ClientId) : IApplicationCommandResult;
}