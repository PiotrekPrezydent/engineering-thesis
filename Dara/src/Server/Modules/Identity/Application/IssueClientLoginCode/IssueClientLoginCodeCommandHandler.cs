using Dara.Server.BuildingBlocks.Application.Commands;
using Dara.Server.Modules.Identity.Domain.Users;
using Dara.Server.Modules.Identity.Domain.Users.Clients;

namespace Dara.Server.Modules.Identity.Application.IssueClientLoginCode;

// a client of another account looks like an unknown one; a new code replaces the previous one of the client
public class IssueClientLoginCodeCommandHandler : ICommandHandler<IssueClientLoginCodeCommand, ClientCodeResult>
{
    private readonly IUserRepository _users;
    private readonly IClientCodeFactory _codeFactory;
    private readonly IdentityOptions _options;
    private readonly TimeProvider _timeProvider;

    public IssueClientLoginCodeCommandHandler(IUserRepository users, IClientCodeFactory codeFactory, IdentityOptions options, TimeProvider timeProvider)
    {
        _users = users;
        _codeFactory = codeFactory;
        _options = options;
        _timeProvider = timeProvider;
    }

    public async Task<ClientCodeResult> HandleAsync(IssueClientLoginCodeCommand command)
    {
        var user = await _users.GetByIdAsync(new UserId(command.UserId));
        var clientId = new UserClientId(command.ClientId);
        var client = user.Clients.FirstOrDefault(c => c.Id == clientId);
        if (client == null)
            return ClientCodeResult.Failed(AuthError.ClientNotFound);

        if (client.IsRevoked)
            return ClientCodeResult.Failed(AuthError.ClientRevoked);

        var now = _timeProvider.GetUtcNow().UtcDateTime;
        var lifetime = _options.ClientLoginCodeLifetime;
        var code = _codeFactory.Create();

        user.IssueClientLoginCode(clientId, code.Hash, now, lifetime);
        return ClientCodeResult.Issued(code.Code, now + lifetime);
    }
}
