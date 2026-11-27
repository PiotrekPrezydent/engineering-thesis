namespace Dara.Server.Modules.Authorization.Domain.Roles;

public interface IRoleExistenceChecker
{
    bool Exists(string roleCode);
}
