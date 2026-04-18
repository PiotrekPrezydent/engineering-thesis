using Dara.BuildingBlocks.Domain;
using Dara.Modules.Communication.Domain.Clients;

namespace Dara.Modules.Communication.Domain.Groups;

public interface IGroupRepository : IRepository<Group>
{
    public Task<Group> GetByNameAsync(GroupName groupName);
    
    public Task<IEnumerable<Client>> GetMembersAsync(Group group);
    
    public Task SaveAsync(Group group);
}