using Dara.BuildingBlocks.Application.Abstraction;

namespace Dara.Modules.Communication.Application.Clients.DeleteClient;

public record DeleteClientCommandResult(Guid DeletedClientId) : IApplicationCommandResult;