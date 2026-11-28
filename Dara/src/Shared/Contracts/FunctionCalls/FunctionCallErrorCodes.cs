namespace Dara.Shared.Contracts.FunctionCalls;

// the "code" of a refused result: 400 a body that breaks the rules of FunctionCallResultRequest, 404 an unknown,
// expired or foreign call (they look the same), 409 a result taken already
public static class FunctionCallErrorCodes
{
    public const string InvalidResult = "invalid_function_call_result";
    public const string UnknownCall = "unknown_function_call";
    public const string AlreadyCompleted = "function_call_completed";
}
