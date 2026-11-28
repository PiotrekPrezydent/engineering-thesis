namespace Dara.Shared.Contracts.FunctionCalls;

// Succeeded needs ResultJson with valid json (any value, "null" too), Failed and NotExecuted need Error; lengths in
// FunctionCallLimits
public record FunctionCallResultRequest(FunctionCallOutcome Outcome, string? ResultJson, string? Error);
