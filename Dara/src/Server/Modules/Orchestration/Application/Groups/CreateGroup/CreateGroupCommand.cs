using Dara.Server.BuildingBlocks.Application.Commands;

namespace Dara.Server.Modules.Orchestration.Application.Groups.CreateGroup;

public record CreateGroupCommand(Guid GroupId, Guid CreatorId) : ICommand;