namespace Dara.Server.Modules.Groups.Application.Groups.GetGroupDetails;

public record GroupDetailsDto(Guid GroupId, Guid OwnerId, string GroupName, string JoinCode, List<Guid> Members);