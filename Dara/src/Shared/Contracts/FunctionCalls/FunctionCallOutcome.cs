namespace Dara.Shared.Contracts.FunctionCalls;

public enum FunctionCallOutcome
{
    // the function returned, ResultJson holds its value (null for a function without a result)
    Succeeded,
    // the function threw, Error says what
    Failed,
    // the client did not run it: no package, another hash, wrong arguments or the client refused; Error says why
    NotExecuted
}
