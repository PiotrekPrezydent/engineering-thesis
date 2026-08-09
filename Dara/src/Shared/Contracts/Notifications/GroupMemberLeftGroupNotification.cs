using Dara.Shared.Contracts.StateSnapshots;

namespace Dara.Shared.Contracts.Notifications;

public record GroupMemberLeftGroupNotification(Guid GroupId, GroupMemberSnapshot GroupMember);