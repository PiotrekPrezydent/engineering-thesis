namespace Dara.Server.Modules.Authorization.Application.Users.CheckPermissions;

public record PermissionCheckDto(string PermissionCode, bool IsAllowed);
