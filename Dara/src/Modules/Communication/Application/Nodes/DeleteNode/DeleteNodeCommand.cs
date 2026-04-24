using Dara.BuildingBlocks.Application.Abstraction;

namespace Dara.Modules.Communication.Application.Nodes.DeleteNode;

public record DeleteNodeCommand(Guid ClientId) : IApplicationCommand;