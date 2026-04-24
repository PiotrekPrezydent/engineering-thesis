using Dara.BuildingBlocks.Application.Abstraction;

namespace Dara.Modules.Communication.Application.Connections.DeleteConnection;

public record DeleteConnectionCommand(string ConnectionId) : IApplicationCommand;