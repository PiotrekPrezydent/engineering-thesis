using Dara.BuildingBlocks.Application.Abstraction;

namespace Dara.Modules.Communication.Application.Clients.ChangeClientAuthToken;

public record ChangeClientAuthTokenCommand(Guid ClientId, string NewAuthToken) : IApplicationCommand;