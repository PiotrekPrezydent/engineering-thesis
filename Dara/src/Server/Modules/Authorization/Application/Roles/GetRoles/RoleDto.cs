namespace Dara.Server.Modules.Authorization.Application.Roles.GetRoles;

public record RoleDto(string Code, string Module, bool IsDefault, List<string> PermissionCodes);
