using System.Security.Claims;
using System.Text.Json;
using Dara.Server.Apps.API.Common.Authentication;
using Dara.Server.Apps.API.Common.Errors;
using Dara.Shared.Contracts.FunctionCalls;
using Microsoft.AspNetCore.Mvc;

namespace Dara.Server.Apps.API.Common.FunctionCalls;

// the result of a call from the client the call went to (client token); the body is checked first, then the call
public static class FunctionCallEndpoints
{
    public static IEndpointRouteBuilder MapFunctionCallEndpoints(this IEndpointRouteBuilder app)
    {
        app.MapGroup(FunctionCallRoutes.Group)
            .RequireAuthorization(AuthPolicies.Client)
            .MapPost("/{callId:guid}/result", SubmitResultAsync);

        return app;
    }

    private static async Task<IResult> SubmitResultAsync(Guid callId, FunctionCallResultRequest request, ClaimsPrincipal user,
        [FromServices] PendingFunctionCalls pending, [FromServices] IFunctionCallResultPort port, [FromServices] TimeProvider time,
        CancellationToken cancellationToken)
    {
        if (Invalid(request) is { } reason)
            return DomainProblems.Problem(StatusCodes.Status400BadRequest, FunctionCallErrorCodes.InvalidResult, reason);

        var clientId = user.ClientId();
        switch (pending.TryComplete(callId, clientId))
        {
            case CompletionResult.Unknown or CompletionResult.WrongClient:
                return DomainProblems.Problem(StatusCodes.Status404NotFound, FunctionCallErrorCodes.UnknownCall, $"no call {callId} waits for this client");
            case CompletionResult.AlreadyCompleted:
                return DomainProblems.Problem(StatusCodes.Status409Conflict, FunctionCallErrorCodes.AlreadyCompleted, $"the result of call {callId} was taken already");
        }

        await port.AcceptAsync(new FunctionCallResult(callId, clientId, FunctionCallResult.From(request.Outcome), request.ResultJson, request.Error,
            time.GetUtcNow().UtcDateTime), cancellationToken);
        return TypedResults.NoContent();
    }

    private static string? Invalid(FunctionCallResultRequest request)
    {
        if (!Enum.IsDefined(request.Outcome))
            return "unknown outcome";

        if (request.Outcome == FunctionCallOutcome.Succeeded)
        {
            if (request.ResultJson is null)
                return "a succeeded call needs ResultJson (\"null\" for a function without a result)";
            if (request.ResultJson.Length > FunctionCallLimits.MaxResultJsonLength)
                return $"ResultJson is longer than {FunctionCallLimits.MaxResultJsonLength}";
            try
            {
                using var _ = JsonDocument.Parse(request.ResultJson);
            }
            catch (JsonException)
            {
                return "ResultJson is no valid json";
            }

            return null;
        }

        if (string.IsNullOrWhiteSpace(request.Error))
            return $"a call with the outcome {request.Outcome} needs Error";
        return request.Error.Length > FunctionCallLimits.MaxErrorLength ? $"Error is longer than {FunctionCallLimits.MaxErrorLength}" : null;
    }
}
