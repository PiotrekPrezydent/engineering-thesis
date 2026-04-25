using Dara.BuildingBlocks.Application.Abstraction;

namespace Dara.Modules.Communication.Application.Clients.ChangeClientName;

public record ChangeClientNameCommand(Guid ClientId, string NewName) : IApplicationCommand;