namespace Dara.Server.Modules.Authorization.Application.Users;

// the roles of the caller and every permission they allow together
public record CallerPermissionsDto(List<string> RoleCodes, List<string> PermissionCodes);
