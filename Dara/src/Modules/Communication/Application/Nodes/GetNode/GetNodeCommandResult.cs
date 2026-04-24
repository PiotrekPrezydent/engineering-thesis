using Dara.BuildingBlocks.Application.Abstraction;

namespace Dara.Modules.Communication.Application.Nodes.GetNode;

public record GetNodeCommandResult(Guid NodeId, string NodeName, string NodeAuthToken) : IApplicationCommandResult;