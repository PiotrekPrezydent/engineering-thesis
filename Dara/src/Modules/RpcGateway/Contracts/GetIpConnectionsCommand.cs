using Dara.BuildingBlocks.Contracts;

namespace Dara.Modules.RpcGateway.Contracts;

public record GetIpConnectionsCommand(string Ip) : IApplicationCommand;

public record GetIpConnectionsCommandResult(IEnumerable<string> ConnectionIds) : IApplicationCommandResult;
