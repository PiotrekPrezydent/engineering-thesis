using Dara.Server.BuildingBlocks.Application.Commands;

namespace Dara.Server.Modules.Groups.Application.SendMessageToGroup;

public record SendMessageToGroupCommand(Guid GroupId, Guid SenderId, string Content) : ICommand;