using Dara.BuildingBlocks.Application.Abstraction;

namespace Dara.Modules.Connections.Application.Clients.ChangeClientName
{
    public record ChangeClientNameCommand(Guid ClientId, string NewName) : IApplicationCommand;
}