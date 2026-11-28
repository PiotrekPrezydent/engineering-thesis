namespace Dara.Shared.Contracts.FunctionCalls;

// the result of a call goes back over http with the client token of the client the call went to
public static class FunctionCallRoutes
{
    public const string Group = "/function-calls";

    public static string Result(Guid callId) => $"{Group}/{callId}/result";
}
