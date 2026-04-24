using Dara.BuildingBlocks.Application.Abstraction;

namespace Dara.Modules.Communication.Application.Nodes.GetNode;

public record GetNodeCommand(Guid ClientId) : IApplicationCommand;