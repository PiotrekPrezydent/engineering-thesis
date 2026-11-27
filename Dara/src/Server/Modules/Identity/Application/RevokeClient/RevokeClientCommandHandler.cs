using Dara.Server.BuildingBlocks.Application.Commands;
using Dara.Server.Modules.Identity.Application.Clients;
using Dara.Server.Modules.Identity.Domain.Clients;
using Dara.Server.Modules.Identity.Domain.UserSessions;

namespace Dara.Server.Modules.Identity.Application.RevokeClient;

// the client takes no change any more, its open login code goes and its session ends with its refresh tokens, all
// in one unit of work; the host closes its hub connections on UserClientRevoked; the panel client cannot be revoked
public class RevokeClientCommandHandler : ICommandHandler<RevokeClientCommand>
{
    private readonly IClientRepository _clients;
    private readonly IUserSessionRepository _sessions;
    private readonly TimeProvider _timeProvider;

    public RevokeClientCommandHandler(IClientRepository clients, IUserSessionRepository sessions, TimeProvider timeProvider)
    {
        _clients = clients;
        _sessions = sessions;
        _timeProvider = timeProvider;
    }

    public async Task HandleAsync(RevokeClientCommand command)
    {
        var now = _timeProvider.GetUtcNow().UtcDateTime;

        var client = await _clients.GetAsync(command.UserId, command.ClientId);
        client.Revoke(now);

        var session = await _sessions.FindByClientIdAsync(client.Id);
        session?.Revoke(now);
    }
}
