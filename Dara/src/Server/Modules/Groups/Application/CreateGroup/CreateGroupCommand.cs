using Dara.Server.BuildingBlocks.Application.Commands;

namespace Dara.Server.Modules.Groups.Application.CreateGroup;

public record CreateGroupCommand(Guid CreatorId, string GroupName) : ICommand<Guid>;