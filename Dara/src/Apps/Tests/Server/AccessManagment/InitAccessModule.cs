using Dara.BuildingBlocks.Domain.Commands;
using Dara.BuildingBlocks.Domain.Events;
using Dara.Modules.AccessManagment.Application.Clients;
using Dara.Modules.AccessManagment.Application.Devices;
using Dara.Modules.AccessManagment.Application.Domain;
using Dara.Modules.AccessManagment.Application.User;
using Dara.Modules.AccessManagment.Domain.Auth;
using Dara.Modules.AccessManagment.Domain.Clients;
using Dara.Modules.AccessManagment.Domain.Clients.Events;
using Dara.Modules.AccessManagment.Domain.Devices;
using Dara.Modules.AccessManagment.Domain.Devices.Events;
using Dara.Modules.AccessManagment.Domain.Users;
using Dara.Modules.AccessManagment.Domain.Users.Events;
using Dara.Modules.AccessManagment.Infrastructure;

namespace Dara.Apps.Tests.Server.AccessManagment;

public static class InitAccessModule
{
    public static void AddAccessModule(this IServiceCollection services)
    {
        services.AddScoped<IApplicationCommandHandler<AddClientCommand, AddClientCommandResult>, AddClientCommandHandler>();
        services.AddScoped<IApplicationCommandHandler<GetClientCommand, GetClientCommandResult>, GetClientCommandHandler>();
        services.AddScoped<IApplicationCommandHandler<LoginClientUserCommand, LoginClientUserCommandResult>, LoginClientUserCommandHandler>();
        services.AddScoped<IApplicationCommandHandler<LogoutClientUserCommand, LogoutClientUserCommandResult>, LogoutClientUserCommandHandler>();
        services.AddScoped<IApplicationCommandHandler<RegisterClientDeviceCommand, RegisterClientDeviceCommandResult>, RegisterClientDeviceCommandHandler>();
        services.AddScoped<IApplicationCommandHandler<AddDeviceCommand, AddDeviceCommandResult>, AddDeviceCommandHandler>();
        services.AddScoped<IApplicationCommandHandler<GetDeviceCommand, GetDeviceCommandResult>, GetDeviceCommandHandler>();
        services.AddScoped<IApplicationCommandHandler<AddUserCommand, AddUserCommandResult>, AddUserCommandHandler>();
        services.AddScoped<IApplicationCommandHandler<GetUserCommand, GetUserCommandResult>, GetUserCommandHandler>();


        services.AddScoped<IDomainEventHandler<ClientCreatedEvent>, ClientCreatedEventHandler>();
        services.AddScoped<IDomainEventHandler<ClientUserAddedEvent>, ClientUserAddedEventHandler>();
        services.AddScoped<IDomainEventHandler<ClientUserRemovedEvent>, ClientUserRemovedEventHandler>();
        services.AddScoped<IDomainEventHandler<DeviceCreatedEvent>, DeviceCreatedEventHandler>();
        services.AddScoped<IDomainEventHandler<UserCreatedEvent>, UserCreatedEventHandler>();

        services.AddScoped<IClientRepository, ClientRepository>();
        services.AddScoped<IDeviceRepository, DeviceRepository>();
        services.AddScoped<IUserRepository, UserRepository>();

        services.AddScoped<IPasswordHasher, MockPasswordHasher>();
    }
}