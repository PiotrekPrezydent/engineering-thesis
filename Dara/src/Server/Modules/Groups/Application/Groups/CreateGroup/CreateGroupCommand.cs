using Dara.Server.BuildingBlocks.Application.Commands;

namespace Dara.Server.Modules.Groups.Application.Groups.CreateGroup;

public record CreateGroupCommand(Guid CreatorId, string GroupName, string JoinCode) : ICommand<Guid>;