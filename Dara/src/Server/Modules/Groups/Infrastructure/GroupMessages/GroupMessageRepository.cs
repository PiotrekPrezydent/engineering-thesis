using Dara.Server.Modules.Groups.Domain.GroupMessages;
using Microsoft.EntityFrameworkCore;

namespace Dara.Server.Modules.Groups.Infrastructure.GroupMessages;

public class GroupMessageRepository : IGroupMessageRepository
{
    private GroupContext _contextBase;

    public GroupMessageRepository(GroupContext contextBase)
    {
        _contextBase = contextBase;
    }

    public async Task AddAsync(GroupMessage groupMessage)
    {
        await _contextBase.GroupMessages.AddAsync(groupMessage);
    }

    public async Task<GroupMessage> GetByIdAsync(GroupMessageId groupMessageId)
    {
        return await _contextBase.GroupMessages.FirstAsync(e=>e.MessageId == groupMessageId);
    }
}