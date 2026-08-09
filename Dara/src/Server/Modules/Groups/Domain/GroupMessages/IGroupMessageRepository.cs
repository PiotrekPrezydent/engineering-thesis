using Dara.Server.BuildingBlocks.Domain;

namespace Dara.Server.Modules.Groups.Domain.GroupMessages;

public interface IGroupMessageRepository : IRepository
{
    public Task AddAsync(GroupMessage groupMessage);
    
    public Task<GroupMessage> GetByIdAsync(GroupMessageId groupMessageId);
}