using Dara.BuildingBlocks.Application.Abstraction;

namespace Dara.Modules.Communication.Application.Nodes.ChangeNodeName;

public record ChangeNodeNameCommand(Guid NodeId, string NewName) : IApplicationCommand;