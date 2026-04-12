using Dara.API.ModulesBuilder;
using Dara.API.Server.Auth;
using Dara.Core.Domain.Commands;
using Dara.Core.Domain.Events;
using Dara.Core.Infrastructure;
using Dara.Modules.AccessManagment.Application.Auth;
using Dara.Modules.AccessManagment.Domain.Auth;
using Dara.Modules.AccessManagment.Domain.User;
using Dara.Modules.AccessManagment.Infrastructure;

namespace Dara.API.Server;

public class Program
{
    public static async Task Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);
        builder.Services.AddSignalR();
        
        //core inra
        CoreModule core = new();
        core.AddToServices(builder.Services);
        
        //access module
        AccessManagmentModule mod = new();
        mod.AddToServices(builder.Services);
        
        builder.Services.AddTransient<AuthHub>();

        var app = builder.Build();
        app.MapHub<AuthHub>("auth");

        app.Run();
    }
}


