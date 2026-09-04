using Dara.Server.BuildingBlocks.Application.Commands;

namespace Dara.Server.Modules.Orchestration.Application.Groups.EditApiKey;

public record EditApiKeyCommand(Guid GroupId, string ApiKey) : ICommand;