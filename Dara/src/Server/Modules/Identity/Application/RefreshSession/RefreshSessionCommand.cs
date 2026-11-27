using Dara.Server.BuildingBlocks.Application.Commands;

namespace Dara.Server.Modules.Identity.Application.RefreshSession;

// only clients have refresh tokens; the account panel keeps its session in a cookie
public record RefreshSessionCommand(string RefreshToken) : ICommand<ClientTokensResult>;
