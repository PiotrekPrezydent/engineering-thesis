using Dara.Server.BuildingBlocks.Domain;
using Dara.Server.BuildingBlocks.Domain.Exceptions;
using Dara.Server.Modules.Authorization.Domain.RoleAssignments.Rules;
using Dara.Server.Modules.Authorization.Domain.Roles;
using Dara.Server.Modules.Authorization.Domain.Roles.Rules;

namespace Dara.Server.Modules.Authorization.Application.RoleAssignments;

// a grant or revoke through the api: the role on its resource and its companions on theirs, each checked by the same
// rules on its own resource, so Authorization needs no knowledge of which function belongs to which plugin
internal static class Companions
{
    public static async Task<List<(Role Role, Guid ScopeId)>> ResolveAsync(IRoleRepository roles, string roleCode, Guid scopeId,
        IReadOnlyList<CompanionAssignment>? companions)
    {
        var role = await roles.FindByCodeAsync(roleCode);
        Check(new RoleMustExistRule(role));
        var resolved = new List<(Role Role, Guid ScopeId)> { (role!, scopeId) };

        foreach (var companion in (companions ?? []).Distinct())
        {
            Check(new CompanionMustBelongToTheRoleRule(role!, companion.RoleCode));
            var companionRole = await roles.FindByCodeAsync(companion.RoleCode);
            Check(new RoleMustExistRule(companionRole));
            resolved.Add((companionRole!, companion.ScopeId));
        }

        return resolved;
    }

    public static void Check(IBuisnessRule rule)
    {
        if (rule.IsBroken())
            throw new BuisnessRuleValidationException(rule);
    }
}
