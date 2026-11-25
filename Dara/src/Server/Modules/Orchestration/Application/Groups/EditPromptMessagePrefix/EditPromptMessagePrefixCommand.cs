using Dara.Server.BuildingBlocks.Application.Commands;

namespace Dara.Server.Modules.Orchestration.Application.Groups.EditPromptMessagePrefix;

public record EditPromptMessagePrefixCommand(Guid GroupId, Guid RequesterId, string PromptMessagePrefix) : ICommand;
