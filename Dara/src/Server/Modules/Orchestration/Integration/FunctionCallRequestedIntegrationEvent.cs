using Dara.Server.BuildingBlocks.Integration;

namespace Dara.Server.Modules.Orchestration.Integration;

// orchestration asks the host to call a function on one client (through the outbox, with the step saved); the host
// sends it when the client is online and reports NotDelivered otherwise. TASK-ENG-013 publishes it, the host keeps the
// call until ExpiresAt, so a result after that is unknown
public record FunctionCallRequestedIntegrationEvent : IntegrationEventBase
{
    public Guid CallId { get; }
    public Guid AgentRunId { get; }
    public Guid TargetClientId { get; }
    public string PluginName { get; }
    public string PluginHash { get; }
    public string FunctionName { get; }
    public string ArgumentsJson { get; }
    public DateTime ExpiresAt { get; }

    public FunctionCallRequestedIntegrationEvent(Guid eventId, DateTime occurredOn, Guid callId, Guid agentRunId, Guid targetClientId,
        string pluginName, string pluginHash, string functionName, string argumentsJson, DateTime expiresAt) : base(eventId, occurredOn)
    {
        CallId = callId;
        AgentRunId = agentRunId;
        TargetClientId = targetClientId;
        PluginName = pluginName;
        PluginHash = pluginHash;
        FunctionName = functionName;
        ArgumentsJson = argumentsJson;
        ExpiresAt = expiresAt;
    }
}
