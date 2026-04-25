using Dara.BuildingBlocks.Application.Commands;

namespace Dara.Modules.Connections.Application.Clients.ChangeClientAuthToken
{
    public record ChangeClientAuthTokenCommand(Guid ClientId, string NewAuthToken) : IModuleCommand;
}