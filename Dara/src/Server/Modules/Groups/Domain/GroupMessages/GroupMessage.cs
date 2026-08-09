using Dara.Server.BuildingBlocks.Domain;
using Dara.Server.Modules.Groups.Domain.GroupMessages.Events;
using Dara.Server.Modules.Groups.Domain.Groups;

namespace Dara.Server.Modules.Groups.Domain.GroupMessages;

public class GroupMessage : Entity, IAggregateRoot
{
    public GroupMessageId Id { get; init; }
    
    public GroupId GroupId { get; init; }
    
    public GroupMemberId Sender { get; init; }
    
    public string Content { get; init; }

    private GroupMessage(GroupId groupId, GroupMemberId sender, string content)
    {
        Id = new GroupMessageId(Guid.NewGuid());
        GroupId = groupId;
        Sender = sender;
        Content = content;
        AddDomainEvent(new NewGroupMessageCreated(GroupId,Sender,Content));
    }

    public static GroupMessage Create(GroupId groupId, GroupMemberId sender, string content)
    {
        return new GroupMessage(groupId, sender, content);
    }
    
    
}