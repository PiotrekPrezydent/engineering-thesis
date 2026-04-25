using Dara.BuildingBlocks.Application.Abstraction;

namespace Dara.Modules.Communication.Application.Clients.GetClient;

public record GetClientCommandResult(Guid ClientId) : IApplicationCommandResult;