using Dara.Server.BuildingBlocks.Application.Commands;
using Dara.Server.BuildingBlocks.Application.Queries;
using Dara.Server.BuildingBlocks.Infrastructure.DataAccess;
using Dara.Server.BuildingBlocks.Infrastructure.Messaging.EventBus;
using Dara.Server.Modules.Identity.Application;
using Dara.Server.Modules.Identity.Application.IssueDeviceLoginCode;
using Dara.Server.Modules.Identity.Application.RegisterDeviceSession;
using Dara.Server.Modules.Identity.Application.RegisterUser;
using Dara.Server.Modules.Identity.Application.SignIn;
using Dara.Server.Modules.Identity.Infrastructure;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Dara.Server.Modules.Identity.IntegrationTests;

// the identity module alone with its outbox loop
public class IdentityModuleHost : IAsyncDisposable
{
    public const string Password = "test-password-1";

    private readonly ServiceProvider _services;
    private readonly IReadOnlyList<IHostedService> _workers;

    public InMemoryEventBus EventBus { get; } = new();

    public IdentityCompositionRoot Root { get; }

    private IdentityModuleHost(IdentityOptions options)
    {
        Root = new IdentityCompositionRoot(options);

        var services = new ServiceCollection();
        Root.Initialize(services, EventBus);

        _services = services.BuildServiceProvider();
        _workers = _services.GetServices<IHostedService>().ToList();
    }

    public static async Task<IdentityModuleHost> StartAsync(CancellationToken cancellationToken, IdentityOptions? options = null)
    {
        var host = new IdentityModuleHost(options ?? new IdentityOptions());
        foreach (var worker in host._workers)
            await worker.StartAsync(cancellationToken);

        return host;
    }

    // every call has its own scope, like a call of the api
    public async Task ExecuteAsync<TCommand>(TCommand command) where TCommand : ICommand
    {
        await using var scope = _services.CreateAsyncScope();
        await scope.ServiceProvider.GetRequiredService<IIdentityModule>().ExecuteCommandAsync(command);
    }

    public async Task<TResult> ExecuteAsync<TCommand, TResult>(TCommand command) where TCommand : ICommand<TResult>
    {
        await using var scope = _services.CreateAsyncScope();
        return await scope.ServiceProvider.GetRequiredService<IIdentityModule>().ExecuteCommandAsync<TCommand, TResult>(command);
    }

    public async Task<TResult> QueryAsync<TQuery, TResult>(TQuery query) where TQuery : IQuery<TResult>
    {
        await using var scope = _services.CreateAsyncScope();
        return await scope.ServiceProvider.GetRequiredService<IIdentityModule>().ExecuteQueryAsync<TQuery, TResult>(query);
    }

    // reads module state that the module api has no query for
    public async Task<TResult> ReadAsync<TResult>(Func<IReadModel, TResult> read)
    {
        await using var scope = Root.CreateAsyncScope();
        return read(scope.ServiceProvider.GetRequiredService<IReadModel>());
    }

    // an account with one device of the same id, like the seed
    public async Task<Guid> RegisterAsync(string login)
    {
        var userId = await ExecuteAsync<RegisterUserCommand, Guid>(new RegisterUserCommand(login, $"{login}@test.local", Password));
        await RegisterDeviceAsync(userId, userId);
        return userId;
    }

    public Task RegisterDeviceAsync(Guid userId, Guid deviceId)
    {
        return ExecuteAsync(new RegisterDeviceSessionCommand(deviceId, userId));
    }

    public Task<DeviceLoginCodeResult> IssueCodeAsync(Guid userId, Guid deviceId)
    {
        return ExecuteAsync<IssueDeviceLoginCodeCommand, DeviceLoginCodeResult>(new IssueDeviceLoginCodeCommand(userId, deviceId));
    }

    public Task<DeviceTokensResult> SignInWithCodeAsync(string code)
    {
        return ExecuteAsync<SignInDeviceCommand, DeviceTokensResult>(new SignInDeviceCommand(code));
    }

    // the panel issues a code and the device signs in with it
    public async Task<DeviceTokensResult> SignInDeviceAsync(Guid userId, Guid deviceId)
    {
        var code = await IssueCodeAsync(userId, deviceId);
        Assert.True(code.Succeeded, $"login code: {code.Error}");
        return await SignInWithCodeAsync(code.Code!);
    }

    public Task<AccountSignInResult> SignInAccountAsync(string login, string password = Password)
    {
        return ExecuteAsync<SignInAccountCommand, AccountSignInResult>(new SignInAccountCommand(login, password));
    }

    public async ValueTask DisposeAsync()
    {
        foreach (var worker in _workers.Reverse())
            await worker.StopAsync(CancellationToken.None);

        await _services.DisposeAsync();
    }
}
