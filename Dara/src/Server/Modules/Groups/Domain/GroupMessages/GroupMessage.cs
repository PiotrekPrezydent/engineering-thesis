using Dara.Server.BuildingBlocks.Domain;
using Dara.Server.Modules.Groups.Domain.GroupMessages.Events;
using Dara.Server.Modules.Groups.Domain.Groups;
using Dara.Server.Modules.Groups.Domain.Members;

namespace Dara.Server.Modules.Groups.Domain.GroupMessages;

public class GroupMessage : Entity, IAggregateRoot
{
    public GroupMessageId Id { get; private set; }
    public GroupId GroupId { get; private set; }
    public MemberId AuthorId { get; private set; }
    
    public string Content { get; init; }

    private GroupMessage(GroupId groupId, MemberId authorId, string content)
    {
        Id = new GroupMessageId(Guid.NewGuid());
        
        GroupId = groupId;
        AuthorId = authorId;
        Content = content;
        
        AddDomainEvent(new NewGroupMessageCreatedDomainEvent(Id, GroupId, AuthorId, Content));
    }

    internal static GroupMessage Create(GroupId groupId, MemberId authorId, string content)
    {
        return new GroupMessage(groupId, authorId, content);
    }
}