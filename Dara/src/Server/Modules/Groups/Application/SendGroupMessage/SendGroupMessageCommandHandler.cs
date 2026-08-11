using Dara.Server.BuildingBlocks.Application.Commands;
using Dara.Server.BuildingBlocks.Domain.Exceptions;
using Dara.Server.BuildingBlocks.Infrastructure.DataAccess;
using Dara.Server.Modules.Groups.Domain.GroupMessages;
using Dara.Server.Modules.Groups.Domain.Groups;
using Microsoft.EntityFrameworkCore;

namespace Dara.Server.Modules.Groups.Application.SendGroupMessage;

public class SendGroupMessageCommandHandler : ICommandHandler<SendGroupMessageCommand>
{
    private readonly IGroupMessageRepository _groupMessageRepository;
    private readonly IReadModel _readModel;
    
    public SendGroupMessageCommandHandler(IReadModel readModel, IGroupMessageRepository groupMessageRepository)
    {
        _readModel = readModel;
        _groupMessageRepository = groupMessageRepository;
    }
    
    public async Task HandleAsync(SendGroupMessageCommand command)
    {
        var groupId = new GroupId(command.GroupId);
        var senderId = new GroupMemberId(command.AuthorId);
        GroupMember? gm = await _readModel.Query<GroupMember>().FirstOrDefaultAsync(e=>e.Id == senderId);
        Group? group = await _readModel.Query<Group>().FirstOrDefaultAsync(e=>e.Id == groupId);

        var rule = new OnlyActualGroupMemberCanSendMessage(group, gm);
        if(gm == null || group == null || rule.IsBroken())
            throw new BuisnessRuleValidationException(rule);
        
        var message = GroupMessage.Create(groupId, senderId,command.Content);
        await _groupMessageRepository.AddAsync(message);
    }
}