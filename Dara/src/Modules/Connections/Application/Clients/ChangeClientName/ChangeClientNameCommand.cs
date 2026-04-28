using Dara.BuildingBlocks.Application;

namespace Dara.Modules.Connections.Application.Clients.ChangeClientName
{
    public record ChangeClientNameCommand(Guid ClientId, string NewName) : IModuleCommand;
}