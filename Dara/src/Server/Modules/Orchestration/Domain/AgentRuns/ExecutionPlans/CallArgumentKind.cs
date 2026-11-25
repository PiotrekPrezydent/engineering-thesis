namespace Dara.Server.Modules.Orchestration.Domain.AgentRuns.ExecutionPlans;

public enum CallArgumentKind
{
    // a json value given by the model
    Value,

    // the whole result of an earlier call, the model wrote {{result:N}}
    Result,

    // a text with results of earlier calls inside
    Text
}
