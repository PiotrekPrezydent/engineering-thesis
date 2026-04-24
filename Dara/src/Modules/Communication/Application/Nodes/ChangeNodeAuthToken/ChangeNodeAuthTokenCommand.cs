using Dara.BuildingBlocks.Application.Abstraction;

namespace Dara.Modules.Communication.Application.Nodes.ChangeNodeAuthToken;

public record ChangeNodeAuthTokenCommand(Guid NodeId, string NewAuthToken) : IApplicationCommand;