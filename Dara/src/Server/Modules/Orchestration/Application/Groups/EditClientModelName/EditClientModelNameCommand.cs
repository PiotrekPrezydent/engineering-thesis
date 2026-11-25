using Dara.Server.BuildingBlocks.Application.Commands;

namespace Dara.Server.Modules.Orchestration.Application.Groups.EditClientModelName;

public record EditClientModelNameCommand(Guid GroupId, Guid RequesterId, string ClientModelName) : ICommand;
