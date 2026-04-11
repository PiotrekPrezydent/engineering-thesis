
using Dara.Core.Domain.Commands;
using Dara.Core.Domain.Events;
using Dara.Core.Infrastructure;
using Dara.Modules.AccessManagment.Application.Auth;
using Dara.Modules.AccessManagment.Domain.Auth;
using Dara.Modules.AccessManagment.Domain.User;
using Dara.Modules.AccessManagment.Infrastructure;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Dara.Modules.AccessManagment.Tests;

class Program
{
    static async Task Main(string[] args)
    {
        var host = Host.CreateDefaultBuilder(args)
            .ConfigureServices((context, services) =>
            {
                //core infra
                services.AddSingleton<IApplicationCommandDispatcher, ApplicationCommandDispatcher>();
                services.AddSingleton<IDomainEventDispatcher, DomainEventDispatcher>();
                
                //app
                services.AddScoped<IApplicationCommandHandler<RegisterUserCommand, RegisterUserCommandResult>, RegisterUserCommandHandler>();
                
                //module infra
                services.AddScoped<IUserRepository, UserRepository>();
                services.AddScoped<IPasswordHasher, MockPasswordHasher>();
                
                services.AddTransient<AccessModuleTest>();
            })
            .Build();
        var app = host.Services.GetRequiredService<AccessModuleTest>();
        await app.RunApp();
    }
}