using Dara.Shared.Contracts.StateSnapshots;

namespace Dara.Shared.Contracts.Notifications;

public record GroupMemberJoinedGroupNotification(Guid GroupId, GroupMemberSnapshot GroupMember);