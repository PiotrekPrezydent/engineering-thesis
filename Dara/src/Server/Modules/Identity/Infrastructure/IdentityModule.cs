using Dara.Server.BuildingBlocks.Infrastructure;
using Dara.Server.BuildingBlocks.Infrastructure.Mediation.HandlerResolving;
using Dara.Server.BuildingBlocks.Infrastructure.Processing.Commands;
using Dara.Server.Modules.Identity.Application;
using Dara.Server.Modules.Identity.Application.AuthenticatePanelSignIn;

namespace Dara.Server.Modules.Identity.Infrastructure;

public class IdentityModule : ModuleBase, IIdentityModule
{
    private readonly PanelSignInAuthenticator _panelSignInAuthenticator;

    public IdentityModule(ICommandExecutor commandExecutor, IHandlersResolver handlersResolver, PanelSignInAuthenticator panelSignInAuthenticator) : base(commandExecutor, handlersResolver)
    {
        _panelSignInAuthenticator = panelSignInAuthenticator;
    }

    public Task<PanelSignInResult?> AuthenticatePanelSignInAsync(Guid panelSignInId)
    {
        return _panelSignInAuthenticator.AuthenticateAsync(panelSignInId);
    }
}
