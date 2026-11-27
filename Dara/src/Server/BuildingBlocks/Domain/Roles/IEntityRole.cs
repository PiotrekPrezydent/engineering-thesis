namespace Dara.Server.BuildingBlocks.Domain.Roles;

// a role of a module or of an entity of a module; the code is the name of the implementing type and the role,
// e.g. "GroupsRole.Owner", so codes of different modules never collide
public interface IEntityRole
{
    string RoleCode { get; }
}
