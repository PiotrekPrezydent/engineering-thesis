using Dara.Server.BuildingBlocks.Application.Commands;
using Dara.Server.Modules.Identity.Domain;

namespace Dara.Server.Modules.Identity.Application.RegisterUser;

// accounts come only from the seed and from tests for now (no registration endpoint); its devices come from
// the account panel (RegisterDeviceSessionCommand)
public class RegisterUserCommandHandler : ICommandHandler<RegisterUserCommand, Guid>
{
    private readonly IUserRepository _userRepository;
    private readonly IPasswordHasher _passwordHasher;
    private readonly IUserUniquenessChecker _uniquenessChecker;
    private readonly TimeProvider _timeProvider;

    public RegisterUserCommandHandler(IUserRepository userRepository, IPasswordHasher passwordHasher, IUserUniquenessChecker uniquenessChecker, TimeProvider timeProvider)
    {
        _userRepository = userRepository;
        _passwordHasher = passwordHasher;
        _uniquenessChecker = uniquenessChecker;
        _timeProvider = timeProvider;
    }

    public async Task<Guid> HandleAsync(RegisterUserCommand command)
    {
        var user = User.Register(command.Login, command.Email, command.Password,
            _timeProvider.GetUtcNow().UtcDateTime, _passwordHasher, _uniquenessChecker);
        await _userRepository.AddAsync(user);

        return user.Id.Value;
    }
}
