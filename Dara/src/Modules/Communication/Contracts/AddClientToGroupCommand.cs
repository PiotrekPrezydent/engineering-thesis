using Dara.BuildingBlocks.Contracts;

namespace Dara.Modules.Communication.Contracts;

public record AddClientToGroupCommand() : IApplicationCommand;

public record AddClientToGroupCommandResult() : IApplicationCommandResult;