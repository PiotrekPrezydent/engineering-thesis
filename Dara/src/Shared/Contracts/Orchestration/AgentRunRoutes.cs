using Dara.Shared.Contracts.Groups;

namespace Dara.Shared.Contracts.Orchestration;

// agent runs with the client token (only the author sees and decides its run, until TASK-ENG-013), agent settings of
// a group with the cookie of the account panel of the owner
public static class AgentRunRoutes
{
    public const string AgentRuns = "/agent-runs";

    public static string AgentRun(Guid agentRunId) => $"{AgentRuns}/{agentRunId}";
    public static string Approve(Guid agentRunId) => $"{AgentRun(agentRunId)}/approve";
    public static string Reject(Guid agentRunId) => $"{AgentRun(agentRunId)}/reject";

    public static string AgentSettings(Guid groupId) => $"{GroupRoutes.AccountGroups}/{groupId}/agent-settings";
    public static string PromptPrefix(Guid groupId) => $"{AgentSettings(groupId)}/prompt-prefix";
    public static string Provider(Guid groupId) => $"{AgentSettings(groupId)}/provider";
    public static string Model(Guid groupId) => $"{AgentSettings(groupId)}/model";
    public static string ApiKey(Guid groupId) => $"{AgentSettings(groupId)}/api-key";
}
