using Dara.BuildingBlocks.Application.Abstraction;

namespace Dara.Modules.Connections.Application.Clients.GetClient
{
    public record GetClientCommandResult(Guid ClientId) : IApplicationCommandResult;
}