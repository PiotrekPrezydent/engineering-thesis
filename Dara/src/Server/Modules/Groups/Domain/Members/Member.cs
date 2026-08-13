using Dara.Server.BuildingBlocks.Domain;
using Dara.Server.Modules.Groups.Domain.Groups;
using Dara.Server.Modules.Groups.Domain.Members.Events;

namespace Dara.Server.Modules.Groups.Domain.Members;

public class Member : Entity, IAggregateRoot
{
    public MemberId Id { get; private set; }
    
    public string Name { get; private set; }

    private Member() { }

    private Member(MemberId id, string name)
    {
        Id = id;
        Name = name;
        AddDomainEvent(new MemberCreatedDomainEvent(Id));
    }

    public static Member CreateDefault(MemberId memberId)
    {
        return new Member(memberId, "DEFAULT-NAME");
    }
    
    public void UpdateName(string newName)
    {
        Name = newName;
        AddDomainEvent(new MemberNameUpdatedDomainEvent(Id, newName));
    }
}