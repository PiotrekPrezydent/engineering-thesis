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

namespace Dara.Apps.Server.API.Initializers;

public static class AccessManagmentModuleInitializer
{
    public static void AddAccessManagmentModule(this IServiceCollection services)
    {
        services.AddSingleton<IApplicationCommandHandler<AddClientCommand, AddClientCommandResult>, AddClientCommandHandler>();
        services.AddSingleton<IApplicationCommandHandler<GetClientCommand, GetClientCommandResult>, GetClientCommandHandler>();
        services.AddSingleton<IApplicationCommandHandler<LoginClientUserCommand, LoginClientUserCommandResult>, LoginClientUserCommandHandler>();
        services.AddSingleton<IApplicationCommandHandler<LogoutClientUserCommand, LogoutClientUserCommandResult>, LogoutClientUserCommandHandler>();
        services.AddSingleton<IApplicationCommandHandler<RegisterClientDeviceCommand, RegisterClientDeviceCommandResult>, RegisterClientDeviceCommandHandler>();
        services.AddSingleton<IApplicationCommandHandler<AddDeviceCommand, AddDeviceCommandResult>, AddDeviceCommandHandler>();
        services.AddSingleton<IApplicationCommandHandler<GetDeviceCommand, GetDeviceCommandResult>, GetDeviceCommandHandler>();
        services.AddSingleton<IApplicationCommandHandler<AddUserCommand, AddUserCommandResult>, AddUserCommandHandler>();
        services.AddSingleton<IApplicationCommandHandler<GetUserCommand, GetUserCommandResult>, GetUserCommandHandler>();


        services.AddSingleton<IDomainEventHandler<ClientCreatedEvent>, ClientCreatedEventHandler>();
        services.AddSingleton<IDomainEventHandler<ClientUserAddedEvent>, ClientUserAddedEventHandler>();
        services.AddSingleton<IDomainEventHandler<ClientUserRemovedEvent>, ClientUserRemovedEventHandler>();
        services.AddSingleton<IDomainEventHandler<DeviceCreatedEvent>, DeviceCreatedEventHandler>();
        services.AddSingleton<IDomainEventHandler<UserCreatedEvent>, UserCreatedEventHandler>();

        services.AddSingleton<IClientRepository, ClientRepository>();
        services.AddSingleton<IDeviceRepository, DeviceRepository>();
        services.AddSingleton<IUserRepository, UserRepository>();

        services.AddSingleton<IPasswordHasher, MockPasswordHasher>();
    }
}