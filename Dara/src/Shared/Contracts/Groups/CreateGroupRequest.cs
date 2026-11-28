namespace Dara.Shared.Contracts.Groups;

// until TASK-ENG-030 a group is owned by a client: a hub client of the signed-in account becomes its owner
public record CreateGroupRequest(string Name, Guid ClientId);
