using Dara.BuildingBlocks.Application.Abstraction;

namespace Dara.Modules.Communication.Application.Nodes.CreateNode;

public record CreateNodeCommandResult(Guid CreatedNodeId) : IApplicationCommandResult;