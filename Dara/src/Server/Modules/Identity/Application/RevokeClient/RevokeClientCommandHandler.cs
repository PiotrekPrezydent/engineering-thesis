using Dara.Server.BuildingBlocks.Application.Commands;
using Dara.Server.Modules.Identity.Domain.ClientSessions;
using Dara.Server.Modules.Identity.Domain.Users;
using Dara.Server.Modules.Identity.Domain.Users.Clients;

namespace Dara.Server.Modules.Identity.Application.RevokeClient;

// the client takes no change any more, its open login code goes and its session ends with its refresh tokens, all
// in one unit of work; the host closes its hub connections on UserClientRevoked
public class RevokeClientCommandHandler : ICommandHandler<RevokeClientCommand>
{
    private readonly IUserRepository _users;
    private readonly IClientSessionRepository _sessions;
    private readonly TimeProvider _timeProvider;

    public RevokeClientCommandHandler(IUserRepository users, IClientSessionRepository sessions, TimeProvider timeProvider)
    {
        _users = users;
        _sessions = sessions;
        _timeProvider = timeProvider;
    }

    public async Task HandleAsync(RevokeClientCommand command)
    {
        var now = _timeProvider.GetUtcNow().UtcDateTime;
        var clientId = new UserClientId(command.ClientId);

        var user = await _users.GetByIdAsync(new UserId(command.UserId));
        user.RevokeClient(clientId, now);

        var session = await _sessions.FindByClientIdAsync(clientId);
        session?.Revoke(now);
    }
}
