using Dara.Server.BuildingBlocks.Application.Commands;

namespace Dara.Server.Modules.Groups.Application.Groups.CreateNewGroup;

public record CreateNewGroupCommand(Guid CreatorId, string GroupName, string JoinCode) : ICommand<Guid>;