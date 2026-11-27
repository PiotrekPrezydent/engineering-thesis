using Dara.Server.BuildingBlocks.Domain;
using Dara.Server.Modules.Groups.Domain.Groups;
using Dara.Server.Modules.Groups.Domain.Members.Events;

namespace Dara.Server.Modules.Groups.Domain.Members;

// a client of an account projected from identity: the id is the id of the client
public class Member : Entity, IAggregateRoot
{
    public MemberId Id { get; private set; }

    public Guid UserId { get; private set; }

    public string Name { get; private set; }

    public bool IsRevoked { get; private set; }

    private Member() { }

    private Member(MemberId id, Guid userId, string name)
    {
        Id = id;
        UserId = userId;
        Name = name;
        AddDomainEvent(new MemberCreatedDomainEvent(Id));
    }

    public static Member Create(MemberId memberId, Guid userId, string name)
    {
        return new Member(memberId, userId, name);
    }

    public void UpdateName(string newName)
    {
        Name = newName;
        AddDomainEvent(new MemberNameUpdatedDomainEvent(Id, newName));
    }

    public void MarkRevoked()
    {
        IsRevoked = true;
    }
}
