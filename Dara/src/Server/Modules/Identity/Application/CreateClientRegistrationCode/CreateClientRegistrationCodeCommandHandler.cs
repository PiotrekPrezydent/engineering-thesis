using Dara.Server.BuildingBlocks.Application.Commands;
using Dara.Server.Modules.Identity.Domain.Users;

namespace Dara.Server.Modules.Identity.Application.CreateClientRegistrationCode;

public class CreateClientRegistrationCodeCommandHandler : ICommandHandler<CreateClientRegistrationCodeCommand, ClientCodeResult>
{
    private readonly IUserRepository _users;
    private readonly IClientCodeFactory _codeFactory;
    private readonly IdentityOptions _options;
    private readonly TimeProvider _timeProvider;

    public CreateClientRegistrationCodeCommandHandler(IUserRepository users, IClientCodeFactory codeFactory, IdentityOptions options, TimeProvider timeProvider)
    {
        _users = users;
        _codeFactory = codeFactory;
        _options = options;
        _timeProvider = timeProvider;
    }

    public async Task<ClientCodeResult> HandleAsync(CreateClientRegistrationCodeCommand command)
    {
        var user = await _users.GetByIdAsync(new UserId(command.UserId));

        var now = _timeProvider.GetUtcNow().UtcDateTime;
        var lifetime = _options.ClientRegistrationCodeLifetime;
        var code = _codeFactory.Create();

        user.CreateClientRegistrationCode(code.Hash, now, lifetime);
        return ClientCodeResult.Issued(code.Code, now + lifetime);
    }
}
