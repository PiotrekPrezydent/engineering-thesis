using Dara.Server.BuildingBlocks.Domain;

namespace Dara.Server.Modules.Orchestration.Domain.Groups;

public interface IGroupRepository : IRepository
{
    public Task AddAsync(Group group);
    
    public Task<Group> GetByIdAsync(GroupId groupId);
}