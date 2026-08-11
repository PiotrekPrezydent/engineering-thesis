using Dara.Server.BuildingBlocks.Infrastructure.DataAccess;
using Dara.Server.Modules.Groups.Domain.Groups;

namespace Dara.Server.Modules.Groups.Infrastructure.Groups;

public static class SeedGroups
{

    public static object[] SeedAllGroupMembers()
    {
        return
        [
            SeedGroupMember(SharedSeedGuids.User1,SharedSeedGuids.Group1),
            SeedGroupMember(SharedSeedGuids.User2,SharedSeedGuids.Group1),
            SeedGroupMember(SharedSeedGuids.User3,SharedSeedGuids.Group1),
            SeedGroupMember(SharedSeedGuids.User4,SharedSeedGuids.Group1),
            SeedGroupMember(SharedSeedGuids.User5,SharedSeedGuids.Group1),
            
            SeedGroupMember(SharedSeedGuids.User2,SharedSeedGuids.Group2),
            SeedGroupMember(SharedSeedGuids.User3,SharedSeedGuids.Group2),
            SeedGroupMember(SharedSeedGuids.User4,SharedSeedGuids.Group2),
            SeedGroupMember(SharedSeedGuids.User5,SharedSeedGuids.Group2),
            
            SeedGroupMember(SharedSeedGuids.User3,SharedSeedGuids.Group3),
            SeedGroupMember(SharedSeedGuids.User4,SharedSeedGuids.Group3),
            SeedGroupMember(SharedSeedGuids.User5,SharedSeedGuids.Group3),
        ];
    }

    public static object[] SeedAllGroups()
    {
        return
        [
            SeedGroup(SharedSeedGuids.Group1,SharedSeedGuids.User1,"G1","G1JC"),
            SeedGroup(SharedSeedGuids.Group2,SharedSeedGuids.User3,"G2","G3JC"),
            SeedGroup(SharedSeedGuids.Group3,SharedSeedGuids.User2,"G3","G2JC"),
        ];
    }
    
    
    public static object SeedGroupMember(Guid memberId, Guid groupId)
    {
        return new
        {
            Id = new GroupMemberId(memberId),
            GroupId = new GroupId(groupId),
        };
    }
    
    public static object SeedGroup(Guid groupId, Guid ownerId, string name, string joinCode)
    {
        return new
        {
            Id = new GroupId(groupId),
            _ownerId = new GroupMemberId(ownerId),
            _name = name,
            _joinCode = joinCode
        };
    }
}