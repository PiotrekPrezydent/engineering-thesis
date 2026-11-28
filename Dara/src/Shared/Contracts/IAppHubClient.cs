using Dara.Shared.Contracts.FunctionCalls;
using Dara.Shared.Contracts.Notifications;
using Dara.Shared.Contracts.StateSnapshots;

namespace Dara.Shared.Contracts;

public interface IAppHubClient
{
    public Task OnClientRenamed(ClientRenamedNotification notification);

    public Task OnClientRevoked(ClientRevokedNotification notification);
    
    public Task OnGroupCreated(GroupCreatedNotification notification);

    public Task OnGroupMemberUpdated(GroupMemberSnapshot groupMemberSnapshot);
    
    public Task OnGroupJoined(GroupSnapshot groupSnapshot);
    
    public Task OnGroupLeft(Guid groupId);
    
    public Task OnGroupMemberJoined(GroupMemberJoinedGroupNotification notification);
    
    public Task OnGroupMemberLeft(GroupMemberLeftGroupNotification notification);

    public Task OnGroupMessageReceived(GroupMessageReceivedNotification notification);

    public Task OnPluginsRegistered(PluginsRegisteredNotification notification);

    // the installations of the client changed (TASK-ENG-031)
    public Task OnClientPluginsChanged(ClientPluginsChangedNotification notification);

    // agent run notifications go only to the author of the prompt
    public Task OnAgentRunStarted(AgentRunStartedNotification notification);

    public Task OnExecutionPlanCreated(ExecutionPlanCreatedNotification notification);

    public Task OnExecutionPlanGenerationFailed(ExecutionPlanGenerationFailedNotification notification);

    public Task OnExecutionPlanApproved(ExecutionPlanApprovedNotification notification);

    public Task OnAgentRunRejected(AgentRunRejectedNotification notification);

    // run a function of a plugin and send the result over http (FunctionCallRoutes.Result) with the same CallId
    public Task CallFunction(FunctionCallRequest request);
}
