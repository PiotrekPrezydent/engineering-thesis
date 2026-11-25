using Dara.Shared.Contracts.Functions;

namespace Dara.Server.Modules.Orchestration.Application.AgentRuns.GenerateExecutionPlan;

// what the model is told; the api key goes only to the provider client, never to a log
public record ExecutionPlanGenerationRequest(
    string ProviderName,
    string ModelName,
    string ApiKey,
    string AuthorName,
    string Prompt,
    IReadOnlyList<string> ClientNames,
    IReadOnlyList<PlanningTool> Tools);

// a participant function under the alias the model calls it by
public record PlanningTool(string Alias, string ClientName, FunctionDescription Function);
