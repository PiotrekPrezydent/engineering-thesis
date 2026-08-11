using Dara.Server.Modules.Groups.Domain.GroupMessages;
using Microsoft.EntityFrameworkCore;

namespace Dara.Server.Modules.Groups.Infrastructure.GroupMessages;

public class GroupMessageRepository : IGroupMessageRepository
{
    private GroupsContext _context;

    public GroupMessageRepository(GroupsContext context)
    {
        _context = context;
    }

    public async Task AddAsync(GroupMessage groupMessage)
    {
        await _context.GroupMessages.AddAsync(groupMessage);
    }

    public async Task<GroupMessage> GetByIdAsync(GroupMessageId groupMessageId)
    {
        return await _context.GroupMessages.FirstAsync(e=>e.Id == groupMessageId);
    }
}