namespace Dara.Shared.Contracts.StateSnapshots;

public record GroupSnapshot(Guid GroupId, Guid GroupOwnerId, string GroupName, string JoinCode, List<GroupMemberSnapshot> GroupMembers);