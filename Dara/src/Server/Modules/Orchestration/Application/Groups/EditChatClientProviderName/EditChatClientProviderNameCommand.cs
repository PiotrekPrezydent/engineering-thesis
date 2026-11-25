using Dara.Server.BuildingBlocks.Application.Commands;

namespace Dara.Server.Modules.Orchestration.Application.Groups.EditChatClientProviderName;

public record EditChatClientProviderNameCommand(Guid GroupId, Guid RequesterId, string ChatClientProviderName) : ICommand;
