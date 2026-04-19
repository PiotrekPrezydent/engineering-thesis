using Dara.BuildingBlocks.Contracts;

namespace Dara.Modules.Communication.Contracts.Client;

public record DestroyClientCommand() : IApplicationCommand;

public record RevokeClientCommandResult() : IApplicationCommandResult;