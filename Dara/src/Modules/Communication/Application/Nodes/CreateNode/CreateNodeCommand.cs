using Dara.BuildingBlocks.Application.Abstraction;

namespace Dara.Modules.Communication.Application.Nodes.CreateNode;

public record CreateNodeCommand(Guid ClientId, string NodeName, string NodeAuthToken) : IApplicationCommand;