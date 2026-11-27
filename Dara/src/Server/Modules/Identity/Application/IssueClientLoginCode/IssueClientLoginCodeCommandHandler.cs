using Dara.Server.BuildingBlocks.Application.Commands;
using Dara.Server.Modules.Identity.Domain.Clients;

namespace Dara.Server.Modules.Identity.Application.IssueClientLoginCode;

// a client of another account and the panel client look like an unknown one; a new code replaces the previous one
public class IssueClientLoginCodeCommandHandler : ICommandHandler<IssueClientLoginCodeCommand, ClientCodeResult>
{
    private readonly IClientRepository _clients;
    private readonly IClientCodeFactory _codeFactory;
    private readonly IdentityOptions _options;
    private readonly TimeProvider _timeProvider;

    public IssueClientLoginCodeCommandHandler(IClientRepository clients, IClientCodeFactory codeFactory, IdentityOptions options, TimeProvider timeProvider)
    {
        _clients = clients;
        _codeFactory = codeFactory;
        _options = options;
        _timeProvider = timeProvider;
    }

    public async Task<ClientCodeResult> HandleAsync(IssueClientLoginCodeCommand command)
    {
        var client = await _clients.FindByIdAsync(new ClientId(command.ClientId));
        if (client == null || client.UserId.Value != command.UserId || client.Kind != ClientKind.Hub)
            return ClientCodeResult.Failed(AuthError.ClientNotFound);

        if (client.IsRevoked)
            return ClientCodeResult.Failed(AuthError.ClientRevoked);

        var now = _timeProvider.GetUtcNow().UtcDateTime;
        var lifetime = _options.ClientLoginCodeLifetime;
        var code = _codeFactory.Create();

        client.IssueLoginCode(code.Hash, now, lifetime);
        return ClientCodeResult.Issued(code.Code, now + lifetime);
    }
}
