using Dara.BuildingBlocks.Application.Abstraction;

namespace Dara.Modules.Communication.Application.Clients.CreateClient;

public record CreateClientCommandResult(Guid ClientId) : IApplicationCommandResult;