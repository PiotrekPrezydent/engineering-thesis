namespace Dara.Shared.Contracts.FunctionCalls;

// the server asks one client to run a function of one of its plugins (IAppHubClient.CallFunction); the client answers
// over http with the same CallId (FunctionCallRoutes.Result). PluginHash is the hash of the package the client must
// have; the shape follows the design of TASK-ENG-031
public record FunctionCallRequest(Guid CallId, string PluginName, string PluginHash, string FunctionName, string ArgumentsJson);
