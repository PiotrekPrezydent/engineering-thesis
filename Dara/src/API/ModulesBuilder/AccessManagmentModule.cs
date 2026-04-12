using Dara.Core.Domain.Commands;
using Dara.Modules.AccessManagment.Application.Auth;
using Dara.Modules.AccessManagment.Domain.Auth;
using Dara.Modules.AccessManagment.Domain.User;
using Dara.Modules.AccessManagment.Infrastructure;
using Microsoft.Extensions.DependencyInjection;

namespace Dara.API.ModulesBuilder;

public class AccessManagmentModule
{
    public AccessManagmentModule()
    {
        
    }
    
    public void AddToServices(IServiceCollection currentServices)
    {
        currentServices.AddScoped<IApplicationCommandHandler<RegisterUserCommand, RegisterUserCommandResult>, RegisterUserCommandHandler>();
        currentServices.AddScoped<IApplicationCommandHandler<LoginUserCommand>, LoginUserCommandHandler>();
            
        currentServices.AddScoped<IUserRepository, UserRepository>();
        currentServices.AddScoped<IPasswordHasher, MockPasswordHasher>();
    }
}