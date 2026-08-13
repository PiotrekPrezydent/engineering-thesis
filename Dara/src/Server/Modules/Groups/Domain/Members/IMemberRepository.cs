using Dara.Server.BuildingBlocks.Domain;

namespace Dara.Server.Modules.Groups.Domain.Members;

public interface IMemberRepository : IRepository
{
    public Task AddAsync(Member member);
    
    public Task<Member> GetByIdAsync(MemberId memberId);
}