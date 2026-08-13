using Dara.Server.Modules.Groups.Domain.Members;
using Microsoft.EntityFrameworkCore;

namespace Dara.Server.Modules.Groups.Infrastructure.Members;

public class MemberRepository : IMemberRepository
{
    private GroupsContext _context;

    public MemberRepository(GroupsContext context)
    {
        _context = context;
    }

    public async Task AddAsync(Member member)
    {
        await _context.Members.AddAsync(member);
    }

    public async Task<Member> GetByIdAsync(MemberId memberId)
    {
        return await _context.Members.FirstAsync(e => e.Id == memberId);
    }
}