using Dara.Server.BuildingBlocks.Application.Queries;

namespace Dara.Server.Modules.Groups.Application.GetAllGroupMembers;

public record GetAllGroupMembersQuery(Guid GroupId) : IQuery<List<GroupMemberDto>>;