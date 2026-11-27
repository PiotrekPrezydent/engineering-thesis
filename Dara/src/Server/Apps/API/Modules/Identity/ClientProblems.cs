using Dara.Server.BuildingBlocks.Domain;
using Dara.Server.BuildingBlocks.Domain.Exceptions;
using Dara.Server.Modules.Identity.Domain.Users.Rules;
using Dara.Shared.Contracts.Clients;

namespace Dara.Server.Apps.API.Modules.Identity;

// broken rules of clients as problem details: a missing client or one of another account 404, a revoked client 409,
// any other rule 400 with its name
public class ClientProblems : IEndpointFilter
{
    public async ValueTask<object?> InvokeAsync(EndpointFilterInvocationContext context, EndpointFilterDelegate next)
    {
        try
        {
            return await next(context);
        }
        catch (BuisnessRuleValidationException exception)
        {
            return From(exception.BrokenRule);
        }
    }

    public static IResult NotFound()
    {
        return Problem(StatusCodes.Status404NotFound, ClientErrorCodes.ClientNotFound, "No such client of this account");
    }

    public static IResult Revoked()
    {
        return Problem(StatusCodes.Status409Conflict, ClientErrorCodes.ClientRevoked, "The client was revoked");
    }

    public static IResult From(IBuisnessRule rule)
    {
        return rule switch
        {
            ClientMustBelongToUserRule => NotFound(),
            ClientMustNotBeRevokedRule => Revoked(),
            _ => Problem(StatusCodes.Status400BadRequest, ClientErrorCodes.RuleBroken, rule.Message, rule.Message)
        };
    }

    public static IResult Problem(int status, string code, string detail, string? rule = null)
    {
        var extensions = new Dictionary<string, object?> { ["code"] = code };
        if (rule != null)
            extensions["rule"] = rule;

        return TypedResults.Problem(detail: detail, statusCode: status, extensions: extensions);
    }
}
