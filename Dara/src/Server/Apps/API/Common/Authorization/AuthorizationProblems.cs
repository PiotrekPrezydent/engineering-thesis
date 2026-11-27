using Dara.Server.BuildingBlocks.Domain;
using Dara.Server.BuildingBlocks.Domain.Exceptions;
using Dara.Server.Modules.Authorization.Domain.RoleAssignments.Rules;
using Dara.Server.Modules.Authorization.Domain.Roles.Rules;
using Dara.Shared.Contracts.Authorization;

namespace Dara.Server.Apps.API.Common.Authorization;

// broken rules of Authorization as problem details: no access management or no role that manages the role 403, a
// global assignment 400, a missing role or assignment 404, a repeated grant 409, any other rule 400 with its name
public class AuthorizationProblems : IEndpointFilter
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

    public static IResult Forbidden(string detail)
    {
        return Problem(StatusCodes.Status403Forbidden, AuthorizationErrorCodes.Forbidden, detail);
    }

    public static IResult From(IBuisnessRule rule)
    {
        return rule switch
        {
            GranterMustManageAccessRule or GranterMustManageTheRoleRule => Forbidden(rule.Message),
            GlobalRoleIsAssignedOnlyAutomaticallyRule => Problem(StatusCodes.Status400BadRequest, AuthorizationErrorCodes.GlobalAssignment, rule.Message),
            RoleMustExistRule => Problem(StatusCodes.Status404NotFound, AuthorizationErrorCodes.RoleNotFound, rule.Message),
            RoleMustBeAssignedRule => Problem(StatusCodes.Status404NotFound, AuthorizationErrorCodes.AssignmentNotFound, rule.Message),
            RoleCannotBeAssignedTwiceRule => Problem(StatusCodes.Status409Conflict, AuthorizationErrorCodes.AlreadyAssigned, rule.Message),
            _ => Problem(StatusCodes.Status400BadRequest, AuthorizationErrorCodes.RuleBroken, rule.Message, rule.GetType().Name)
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
