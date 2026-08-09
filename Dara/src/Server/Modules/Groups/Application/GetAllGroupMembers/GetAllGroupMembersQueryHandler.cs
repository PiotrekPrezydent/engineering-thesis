using Dara.Server.BuildingBlocks.Application.Queries;
using Dara.Server.Modules.Groups.Domain.Groups;
using Microsoft.EntityFrameworkCore;

namespace Dara.Server.Modules.Groups.Application.GetAllGroupMembers;

public class GetAllGroupMembersQueryHandler : IQueryHandler<GetAllGroupMembersQuery, List<GroupMemberDto>>
{
    private readonly IReadModel _readModel;

    public GetAllGroupMembersQueryHandler(IReadModel readModel)
    {
        _readModel = readModel;
    }

    public async Task<List<GroupMemberDto>> HandleAsync(GetAllGroupMembersQuery query)
    {
        var group = await _readModel.Query<Group>().FirstAsync(e => e.GroupId == query.GroupId);
        
        return group.Members.Select(e => new GroupMemberDto(e.MemberId.Value)).ToList();
    }
}