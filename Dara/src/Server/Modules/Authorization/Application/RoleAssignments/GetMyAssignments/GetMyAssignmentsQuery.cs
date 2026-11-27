using Dara.Server.BuildingBlocks.Application.Queries;

namespace Dara.Server.Modules.Authorization.Application.RoleAssignments.GetMyAssignments;

// the assignments of the sender, for the information of a client (not for decisions, those are IsAuthorizedQuery)
public record GetMyAssignmentsQuery(Guid SenderId) : IQuery<List<RoleAssignmentDto>>;
