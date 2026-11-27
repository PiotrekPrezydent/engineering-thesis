using Dara.Server.BuildingBlocks.Infrastructure;
using Dara.Server.BuildingBlocks.Infrastructure.Mediation.HandlerResolving;
using Dara.Server.BuildingBlocks.Infrastructure.Processing.Commands;
using Dara.Server.Modules.Identity.Application;
using Dara.Server.Modules.Identity.Application.AuthenticateAccountSession;

namespace Dara.Server.Modules.Identity.Infrastructure;

public class IdentityModule : ModuleBase, IIdentityModule
{
    private readonly AccountSessionAuthenticator _accountSessionAuthenticator;

    public IdentityModule(ICommandExecutor commandExecutor, IHandlersResolver handlersResolver, AccountSessionAuthenticator accountSessionAuthenticator) : base(commandExecutor, handlersResolver)
    {
        _accountSessionAuthenticator = accountSessionAuthenticator;
    }

    public Task<Guid?> AuthenticateAccountSessionAsync(Guid accountSessionId)
    {
        return _accountSessionAuthenticator.AuthenticateAsync(accountSessionId);
    }
}
