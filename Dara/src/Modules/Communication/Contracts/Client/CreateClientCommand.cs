using Dara.BuildingBlocks.Contracts;

namespace Dara.Modules.Communication.Contracts.Client;

public record CreateClientCommand() : IApplicationCommand;

public record RegisterClientCommandResult() : IApplicationCommandResult;