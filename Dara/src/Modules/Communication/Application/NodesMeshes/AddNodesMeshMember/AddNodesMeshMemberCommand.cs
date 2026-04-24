using Dara.BuildingBlocks.Application.Abstraction;

namespace Dara.Modules.Communication.Application.NodesMeshes.AddNodesMeshMember;

public record AddNodesMeshMemberCommand(Guid MeshId, Guid NodeId) : IApplicationCommand;