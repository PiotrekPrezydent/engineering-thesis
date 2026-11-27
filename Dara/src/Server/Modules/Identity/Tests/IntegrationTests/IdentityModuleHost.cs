using Dara.Server.BuildingBlocks.Application.Commands;
using Dara.Server.BuildingBlocks.Application.Queries;
using Dara.Server.BuildingBlocks.Infrastructure.DataAccess;
using Dara.Server.BuildingBlocks.Infrastructure.Messaging.EventBus;
using Dara.Server.Modules.Identity.Application;
using Dara.Server.Modules.Identity.Application.CreateClientRegistrationCode;
using Dara.Server.Modules.Identity.Application.IssueClientLoginCode;
using Dara.Server.Modules.Identity.Application.RegisterClient;
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

    public async Task<Guid?> AuthenticateAsync(Guid accountSessionId)
    {
        await using var scope = _services.CreateAsyncScope();
        return await scope.ServiceProvider.GetRequiredService<IIdentityModule>().AuthenticateAccountSessionAsync(accountSessionId);
    }

    // reads module state that the module api has no query for
    public async Task<TResult> ReadAsync<TResult>(Func<IReadModel, TResult> read)
    {
        await using var scope = Root.CreateAsyncScope();
        return read(scope.ServiceProvider.GetRequiredService<IReadModel>());
    }

    public Task<Guid> RegisterAsync(string login)
    {
        return ExecuteAsync<RegisterUserCommand, Guid>(new RegisterUserCommand(login, $"{login}@test.local", Password));
    }

    public Task<ClientCodeResult> CreateRegistrationCodeAsync(Guid userId)
    {
        return ExecuteAsync<CreateClientRegistrationCodeCommand, ClientCodeResult>(new CreateClientRegistrationCodeCommand(userId));
    }

    public Task<ClientTokensResult> RegisterClientWithCodeAsync(string code, string? type = "Pc", string? name = null)
    {
        return ExecuteAsync<RegisterClientCommand, ClientTokensResult>(new RegisterClientCommand(code, type, name));
    }

    // the panel creates a registration code and the app of the client registers itself with it
    public async Task<ClientTokensResult> RegisterClientAsync(Guid userId, string? type = "Pc", string? name = null)
    {
        var code = await CreateRegistrationCodeAsync(userId);
        Assert.True(code.Succeeded, $"registration code: {code.Error}");
        var tokens = await RegisterClientWithCodeAsync(code.Code!, type, name);
        Assert.True(tokens.Succeeded, $"registration: {tokens.Error}");
        return tokens;
    }

    public Task<ClientCodeResult> IssueLoginCodeAsync(Guid userId, Guid clientId)
    {
        return ExecuteAsync<IssueClientLoginCodeCommand, ClientCodeResult>(new IssueClientLoginCodeCommand(userId, clientId));
    }

    public Task<ClientTokensResult> SignInWithCodeAsync(string code)
    {
        return ExecuteAsync<SignInClientCommand, ClientTokensResult>(new SignInClientCommand(code));
    }

    // the panel issues a login code and an existing client signs in with it
    public async Task<ClientTokensResult> SignInClientAsync(Guid userId, Guid clientId)
    {
        var code = await IssueLoginCodeAsync(userId, clientId);
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
