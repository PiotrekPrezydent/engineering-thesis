using Dara.BuildingBlocks.Domain.Commands;

namespace Dara.Modules.RpcGateway.Contracts;

public record ChangeClientNameCommand(string Name) : IApplicationCommand;

public record ChangeClientNameCommandResult() : IApplicationCommandResult;