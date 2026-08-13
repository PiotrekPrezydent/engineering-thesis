using Dara.Server.BuildingBlocks.Application.Commands;
using Dara.Server.BuildingBlocks.Infrastructure.DataAccess;
using Dara.Server.Modules.Groups.Domain.GroupMessages;
using Dara.Server.Modules.Groups.Domain.Groups;
using Microsoft.EntityFrameworkCore;

namespace Dara.Server.Modules.Groups.Application.Messages.SendGroupMessage;

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
        Group group = await _readModel.Query<Group>()
            .Include(g => g.Members)
            .FirstAsync(e => e.Id.Match(command.GroupId));

        var message = group.SendMessageToGroup(new(command.AuthorId), command.Content);
        
        await _groupMessageRepository.AddAsync(message);
    }
}