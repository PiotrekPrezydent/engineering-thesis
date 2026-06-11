using Dara.Server.BuildingBlocks.Domain;

namespace Dara.Server.Modules.Groups.Domain.Groups;

public class GroupMessage : Entity<GroupMessageId>, IAggregateRoot
{
    private GroupId _groupId;
    private GroupMemberId _authorId;

    public string Text { get; private set; }

    private GroupMessage(GroupId groupId, GroupMemberId authorId, string text) : base(new GroupMessageId(Guid.NewGuid()))
    {
        _groupId = groupId;
        _authorId = authorId;
        Text = text;
    }

    internal static GroupMessage Create(GroupId groupId, GroupMemberId authorId, string text)
    {
        return new GroupMessage(groupId, authorId, text);
    }
}