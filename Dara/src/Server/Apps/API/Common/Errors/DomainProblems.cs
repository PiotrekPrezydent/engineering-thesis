using Dara.Server.Apps.API.Modules.Groups;
using Dara.Server.Apps.API.Modules.Identity;
using Dara.Server.Apps.API.Modules.Orchestration;
using Dara.Server.Apps.API.Modules.Plugins;
using Dara.Server.BuildingBlocks.Domain;
using Dara.Server.BuildingBlocks.Domain.Exceptions;
using Dara.Server.Modules.Identity.Domain.Clients.Rules;
using Dara.Shared.Contracts.Errors;

namespace Dara.Server.Apps.API.Common.Errors;

// broken rules of the modules as problem details with "code" and "rule": each module of the host says which of its
// rules refuse the caller (403) or the state (409), a rule of a client of the account comes from ClientProblems, a
// missing entity is 404, any other rule 400
public class DomainProblems : IEndpointFilter
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
        catch (EntityNotFoundException exception)
        {
            return NotFound(exception.Message);
        }
    }

    public static IResult From(IBuisnessRule rule)
    {
        return rule switch
        {
            ClientMustBelongToUserRule or ClientMustBeHubClientRule or ClientMustNotBeRevokedRule => ClientProblems.From(rule),
            _ => GroupsProblems.From(rule)
                 ?? OrchestrationProblems.From(rule)
                 ?? PluginsProblems.From(rule)
                 ?? Problem(StatusCodes.Status400BadRequest, ApiErrorCodes.RuleBroken, rule.Message, rule)
        };
    }

    public static IResult NotFound(string detail)
    {
        return Problem(StatusCodes.Status404NotFound, ApiErrorCodes.NotFound, detail);
    }

    public static IResult Forbidden(IBuisnessRule rule) => Problem(StatusCodes.Status403Forbidden, ApiErrorCodes.Forbidden, rule.Message, rule);

    public static IResult Conflict(IBuisnessRule rule) => Problem(StatusCodes.Status409Conflict, ApiErrorCodes.Conflict, rule.Message, rule);

    public static IResult Problem(int status, string code, string detail, IBuisnessRule? rule = null)
    {
        var extensions = new Dictionary<string, object?> { ["code"] = code };
        if (rule != null)
            extensions["rule"] = rule.GetType().Name;

        return TypedResults.Problem(detail: detail, statusCode: status, extensions: extensions);
    }
}
