using Dara.Server.BuildingBlocks.Infrastructure;
using Dara.Server.BuildingBlocks.Infrastructure.Configuration;
using Dara.Server.Modules.Identity.Application;
using Dara.Server.Modules.Identity.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Dara.Server.Modules.Identity.Infrastructure;

public class IdentityModule : ModuleBase, IIdentityModule
{
    public IdentityModule(IModuleCompositionRoot compositionRoot) : base(compositionRoot)
    {
    }
}