using Dara.BuildingBlocks.Application.Commands;

namespace Dara.Modules.Connections.Application.Clients.ChangeClientName
{
    public record ChangeClientNameCommand(Guid ClientId, string NewName) : IModuleCommand;
}