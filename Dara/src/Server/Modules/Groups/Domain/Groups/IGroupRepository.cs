using Dara.Server.BuildingBlocks.Domain;

namespace Dara.Server.Modules.Groups.Domain.Groups;

public interface IGroupRepository : IRepository
{
    Task AddAsync(Group group);
    
    Task<Group> GetByIdAsync(GroupId groupId);
}