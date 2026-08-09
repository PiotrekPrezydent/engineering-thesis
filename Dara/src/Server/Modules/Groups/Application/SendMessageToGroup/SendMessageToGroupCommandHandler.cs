using Dara.Server.BuildingBlocks.Application.Commands;
using Dara.Server.BuildingBlocks.Domain.Exceptions;
using Dara.Server.BuildingBlocks.Infrastructure.DataAccess;
using Dara.Server.Modules.Groups.Domain.GroupMessages;
using Dara.Server.Modules.Groups.Domain.Groups;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualBasic;

namespace Dara.Server.Modules.Groups.Application.SendMessageToGroup;

public class SendMessageToGroupCommandHandler : ICommandHandler<SendMessageToGroupCommand>
{
    private readonly IGroupMessageRepository _groupMessageRepository;
    private readonly IReadModel _readModel;
    
    public SendMessageToGroupCommandHandler(IReadModel readModel, IGroupMessageRepository groupMessageRepository)
    {
        _readModel = readModel;
        _groupMessageRepository = groupMessageRepository;
    }
    
    public async Task HandleAsync(SendMessageToGroupCommand command)
    {
        var groupId = new GroupId(command.GroupId);
        var senderId = new GroupMemberId(command.SenderId);
        GroupMember? gm = await _readModel.Query<GroupMember>().FirstOrDefaultAsync(e=>e.MemberId == senderId);
        Group? group = await _readModel.Query<Group>().FirstOrDefaultAsync(e=>e.GroupId == groupId);

        var rule = new OnlyActualGroupMemberCanSendMessage(group, gm);
        if(gm == null || group == null || rule.IsBroken())
            throw new BuisnessRuleValidationException(rule);
        
        var message = GroupMessage.Create(groupId, senderId,command.Content);
        await _groupMessageRepository.AddAsync(message);
    }
}