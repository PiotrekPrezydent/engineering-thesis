using Dara.Server.BuildingBlocks.Application.Commands;
using Dara.Server.Modules.Identity.Domain.Clients;
using Dara.Server.Modules.Identity.Domain.Users;
using Dara.Server.Modules.Identity.Domain.UserSessions;

namespace Dara.Server.Modules.Identity.Application.RegisterUser;

// accounts come only from the seed and from tests for now (no registration endpoint); the account, its panel client
// and the session of the panel are saved together; clients of the hub register themselves with a code of the panel
// (RegisterClientCommand); the result is the user id, the panel client comes from GetPanelClientIdQuery
public class RegisterUserCommandHandler : ICommandHandler<RegisterUserCommand, Guid>
{
    private readonly IUserRepository _userRepository;
    private readonly IClientRepository _clients;
    private readonly IUserSessionRepository _sessions;
    private readonly IPasswordHasher _passwordHasher;
    private readonly IUserUniquenessChecker _uniquenessChecker;
    private readonly TimeProvider _timeProvider;

    public RegisterUserCommandHandler(IUserRepository userRepository, IClientRepository clients, IUserSessionRepository sessions, IPasswordHasher passwordHasher, IUserUniquenessChecker uniquenessChecker, TimeProvider timeProvider)
    {
        _userRepository = userRepository;
        _clients = clients;
        _sessions = sessions;
        _passwordHasher = passwordHasher;
        _uniquenessChecker = uniquenessChecker;
        _timeProvider = timeProvider;
    }

    public async Task<Guid> HandleAsync(RegisterUserCommand command)
    {
        var now = _timeProvider.GetUtcNow().UtcDateTime;
        var user = User.Register(command.Login, command.Email, command.Password, now, _passwordHasher, _uniquenessChecker);
        await _userRepository.AddAsync(user);

        var panel = Client.CreatePanel(user.Id, now);
        await _clients.AddAsync(panel);
        await _sessions.AddAsync(UserSession.StartPanel(panel.Id, user.Id, now));

        return user.Id.Value;
    }
}
