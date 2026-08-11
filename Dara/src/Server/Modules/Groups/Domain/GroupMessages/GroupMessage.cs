using Dara.Server.BuildingBlocks.Domain;
using Dara.Server.Modules.Groups.Domain.GroupMessages.Events;
using Dara.Server.Modules.Groups.Domain.Groups;

namespace Dara.Server.Modules.Groups.Domain.GroupMessages;

public class GroupMessage : Entity, IAggregateRoot
{
    public GroupMessageId Id { get; init; }
    
    public GroupId GroupId { get; init; }
    
    public GroupMemberId MessageAuthorId { get; init; }
    
    public string Content { get; init; }

    private GroupMessage(GroupId groupId, GroupMemberId messageAuthorId, string content)
    {
        Id = new GroupMessageId(Guid.NewGuid());
        
        GroupId = groupId;
        MessageAuthorId = messageAuthorId;
        Content = content;
        
        AddDomainEvent(new NewGroupMessageCreatedDomainEvent(Id, GroupId, MessageAuthorId, Content));
    }

    internal static GroupMessage Create(GroupId groupId, GroupMemberId messageAuthorId, string content)
    {
        return new GroupMessage(groupId, messageAuthorId, content);
    }
}