using Dara.Server.BuildingBlocks.Infrastructure.Messaging;
using Dara.Server.BuildingBlocks.Infrastructure.Configuration;
using Dara.Server.BuildingBlocks.Infrastructure.DataAccess;
using Dara.Server.BuildingBlocks.Infrastructure.Messaging.EventBus;
using Dara.Server.Modules.Authorization.Application;
using Dara.Server.Modules.Authorization.Infrastructure;
using Dara.Server.Modules.Groups.Application;
using Dara.Server.Modules.Groups.Infrastructure;
using Dara.Server.Modules.Identity.Application;
using Dara.Server.Modules.Identity.Application.CreateClientRegistrationCode;
using Dara.Server.Modules.Identity.Application.IssueClientLoginCode;
using Dara.Server.Modules.Identity.Application.RegisterClient;
using Dara.Server.Modules.Identity.Application.RegisterUser;
using Dara.Server.Modules.Identity.Application.RenameClient;
using Dara.Server.Modules.Identity.Application.SignIn;
using Dara.Server.Modules.Identity.Infrastructure;
using Dara.Server.Modules.Orchestration.Application;
using Dara.Server.Modules.Orchestration.Infrastructure;
using Dara.Server.Modules.Orchestration.Infrastructure.AgentRuns.Generation;
using Dara.Server.Modules.Plugins.Application;
using Dara.Server.Modules.Plugins.Infrastructure;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Dara.Server.SystemTests.Hosting;

// all server modules in memory like in the api host, without signalr, with an own event bus
public class ModulesHost : IAsyncDisposable
{
    public static readonly TimeSpan DefaultTimeout = TimeSpan.FromSeconds(10);
    private static readonly TimeSpan PollInterval = TimeSpan.FromMilliseconds(25);

    // retries in milliseconds, so a test with a failing handler does not wait seconds
    public static readonly MessageProcessingOptions ProcessingOptions = new()
    {
        Retry = new RetryPolicy(TimeSpan.FromMilliseconds(10), TimeSpan.FromMilliseconds(200)),
    };

    private readonly ServiceProvider _services;
    private readonly IReadOnlyList<IHostedService> _workers;

    public InMemoryEventBus EventBus { get; } = new();

    public IdentityCompositionRoot IdentityRoot { get; } = new() { ProcessingOptions = ProcessingOptions };
    public AuthorizationCompositionRoot AuthorizationRoot { get; } = new() { ProcessingOptions = ProcessingOptions };
    public GroupsCompositionRoot GroupsRoot { get; } = new() { ProcessingOptions = ProcessingOptions };
    public PluginsCompositionRoot PluginsRoot { get; } = new() { ProcessingOptions = ProcessingOptions };
    public OrchestrationCompositionRoot OrchestrationRoot { get; }

    public IIdentityModule Identity { get; }
    public IAuthorizationModule Authorization { get; }
    public IGroupsModule Groups { get; }
    public IPluginsModule Plugins { get; }
    public IOrchestrationModule Orchestration { get; }

    // without a scripted model a prompt fails before any provider call, because tests set no api key
    private ModulesHost(IChatClientProvider? chatClientProvider)
    {
        OrchestrationRoot = chatClientProvider is null
            ? new OrchestrationCompositionRoot() { ProcessingOptions = ProcessingOptions }
            : new OrchestrationCompositionRoot([chatClientProvider], new ExecutionPlanGeneratorOptions(TimeoutSeconds: 5)) { ProcessingOptions = ProcessingOptions };

        var services = new ServiceCollection();

        // same order as in the api host
        IModuleCompositionRoot[] roots = [IdentityRoot, AuthorizationRoot, GroupsRoot, PluginsRoot, OrchestrationRoot];
        foreach (var root in roots)
            root.Initialize(services, EventBus);

        _services = services.BuildServiceProvider();
        _workers = _services.GetServices<IHostedService>().ToList();

        Identity = _services.GetRequiredService<IIdentityModule>();
        Authorization = _services.GetRequiredService<IAuthorizationModule>();
        Groups = _services.GetRequiredService<IGroupsModule>();
        Plugins = _services.GetRequiredService<IPluginsModule>();
        Orchestration = _services.GetRequiredService<IOrchestrationModule>();
    }

    public static async Task<ModulesHost> StartAsync(CancellationToken cancellationToken, IChatClientProvider? chatClientProvider = null)
    {
        var host = new ModulesHost(chatClientProvider);
        // like the generic host: StartingAsync of every lifecycle service (the authorization definitions) first
        foreach (var worker in host._workers.OfType<IHostedLifecycleService>())
            await worker.StartingAsync(cancellationToken);

        foreach (var worker in host._workers)
            await worker.StartAsync(cancellationToken);

        return host;
    }

    // an account without clients
    public Task<Guid> CreateUserAsync(string login)
    {
        return ExecuteAsync<RegisterUserCommand, Guid>(new RegisterUserCommand(login, $"{login}@test.local", TestPassword));
    }

    // an account with one client, registered like an app does it; the other modules get the client from
    // UserClientRegisteredIntegrationEvent; UserId and ClientId of the result say who it is
    public async Task<ClientTokensResult> CreateClientAsync(string login, string? name = null, string type = "Pc")
    {
        var userId = await CreateUserAsync(login);
        return await RegisterClientAsync(userId, name, type);
    }

    // the panel creates a registration code and the app of the client registers itself with it
    public async Task<ClientTokensResult> RegisterClientAsync(Guid userId, string? name = null, string type = "Pc")
    {
        var code = await ExecuteAsync<CreateClientRegistrationCodeCommand, ClientCodeResult>(new CreateClientRegistrationCodeCommand(userId));
        var tokens = await ExecuteAsync<RegisterClientCommand, ClientTokensResult>(new RegisterClientCommand(code.Code!, type, name));
        Assert.True(tokens.Succeeded, $"registration of a client: {tokens.Error}");
        return tokens;
    }

    // the account panel renames a client of its account
    public Task RenameClientAsync(Guid userId, Guid clientId, string name)
    {
        return ExecuteAsync(new RenameClientCommand(userId, clientId, name));
    }

    public Task<ClientCodeResult> IssueLoginCodeAsync(Guid userId, Guid clientId)
    {
        return ExecuteAsync<IssueClientLoginCodeCommand, ClientCodeResult>(new IssueClientLoginCodeCommand(userId, clientId));
    }

    // an existing client signs in like the client does: a login code of the panel, then client-login
    public async Task<ClientTokensResult> LoginClientAsync(Guid userId, Guid clientId)
    {
        var code = await IssueLoginCodeAsync(userId, clientId);
        Assert.True(code.Succeeded, $"login code: {code.Error}");
        return await ExecuteAsync<SignInClientCommand, ClientTokensResult>(new SignInClientCommand(code.Code!));
    }

    // one scope per call like one http request, so a command sees what internal commands saved after an earlier one
    public async Task ExecuteAsync<TCommand>(TCommand command) where TCommand : BuildingBlocks.Application.Commands.ICommand
    {
        await using var scope = _services.CreateAsyncScope();
        await scope.ServiceProvider.GetRequiredService<IIdentityModule>().ExecuteCommandAsync(command);
    }

    public async Task<TResult> ExecuteAsync<TCommand, TResult>(TCommand command) where TCommand : BuildingBlocks.Application.Commands.ICommand<TResult>
    {
        await using var scope = _services.CreateAsyncScope();
        return await scope.ServiceProvider.GetRequiredService<IIdentityModule>().ExecuteCommandAsync<TCommand, TResult>(command);
    }

    public const string TestPassword = "test-password-1";

    // reads module state that the module api has no query for
    public static async Task<TResult> ReadAsync<TResult>(IModuleCompositionRoot module, Func<IReadModel, Task<TResult>> read)
    {
        await using var scope = module.CreateAsyncScope();
        return await read(scope.ServiceProvider.GetRequiredService<IReadModel>());
    }

    // polls the condition until it holds, fails with the description when the timeout passes
    public static async Task WaitUntilAsync(Func<Task<bool>> condition, string description, TimeSpan? timeout = null)
    {
        var limit = timeout ?? DefaultTimeout;
        using var timeoutCts = new CancellationTokenSource(limit);

        while (!await condition())
        {
            try
            {
                await Task.Delay(PollInterval, timeoutCts.Token);
            }
            catch (OperationCanceledException)
            {
                throw new TimeoutException($"not met within {limit.TotalSeconds} s: {description}");
            }
        }
    }

    public async ValueTask DisposeAsync()
    {
        foreach (var worker in _workers.Reverse())
            await worker.StopAsync(CancellationToken.None);

        await _services.DisposeAsync();
    }
}
