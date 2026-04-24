using Dara.BuildingBlocks.Application.Abstraction;

namespace Dara.Modules.Communication.Application.Nodes.DeleteNode;

public record DeleteNodeCommandResult(Guid DeletedNodeGuid) : IApplicationCommandResult;