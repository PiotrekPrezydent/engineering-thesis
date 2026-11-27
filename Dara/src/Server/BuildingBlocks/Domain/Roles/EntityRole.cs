namespace Dara.Server.BuildingBlocks.Domain.Roles;

// base of the role types of the modules: sealed record GroupsRole(GroupsRole.Roles Value) : EntityRole<GroupsRole.Roles>(Value)
public abstract record EntityRole<TRoles>(TRoles Value) : IEntityRole, IValueObject where TRoles : struct, Enum
{
    public string RoleCode => $"{GetType().Name}.{Value}";
}
