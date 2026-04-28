using Dara.BuildingBlocks.Application;

namespace Dara.Modules.Connections.Application.Clients.ChangeClientAuthToken
{
    public record ChangeClientAuthTokenCommand(Guid ClientId, string NewAuthToken) : IModuleCommand;
}