namespace Dara.Server.Apps.API.Common.Seeding;

// seed settings from configuration (appsettings, environment variables, .env) and waiting for other modules
public class SeedEnvironment
{
    private static readonly TimeSpan PollInterval = TimeSpan.FromMilliseconds(50);

    public bool Enabled { get; }
    public TimeSpan WaitTimeout { get; }

    // agent settings of seeded groups, an empty key makes every prompt fail before a provider call
    public string AgentProvider { get; }
    public string AgentModel { get; }
    public string AgentApiKey { get; }

    // the password of every seeded account, only in .env; empty with the seed on stops the start
    public string UserPassword { get; }

    public SeedEnvironment(IConfiguration configuration)
    {
        UserPassword = configuration.GetValue("Seed:UserPassword", "")!;
        Enabled = configuration.GetValue("Seed:Enabled", true);
        WaitTimeout = configuration.GetValue("Seed:WaitTimeout", TimeSpan.FromSeconds(10));
        AgentProvider = configuration.GetValue("Seed:AgentProvider", "google")!;
        AgentModel = configuration.GetValue("Seed:AgentModel", "gemini-3.1-flash-lite")!;
        AgentApiKey = configuration.GetValue("Seed:AgentApiKey", "")!;
    }

    // modules get events of other modules asynchronously, so a seeder polls their state instead of a fixed delay
    public async Task WaitUntilAsync(Func<Task<bool>> condition, string description, CancellationToken cancellationToken)
    {
        using var timeoutCts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
        timeoutCts.CancelAfter(WaitTimeout);

        while (!await condition())
        {
            try
            {
                await Task.Delay(PollInterval, timeoutCts.Token);
            }
            catch (OperationCanceledException) when (!cancellationToken.IsCancellationRequested)
            {
                throw new TimeoutException($"seed waited {WaitTimeout.TotalSeconds} s for {description}");
            }
        }
    }
}
