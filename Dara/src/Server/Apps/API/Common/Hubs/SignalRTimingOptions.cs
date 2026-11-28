namespace Dara.Server.Apps.API.Common.Hubs;

// section SignalR: how often the server pings a client and how long it waits for a client that sends nothing; a
// broken client is offline after about ClientTimeoutInterval (TASK-ENG-032-D05: 5 s and 10 s, the defaults of
// asp.net core are 15 s and 30 s). Each timeout is at least twice the keep-alive of the other side
public sealed class SignalRTimingOptions
{
    public const string Section = "SignalR";

    public TimeSpan KeepAliveInterval { get; init; } = TimeSpan.FromSeconds(5);
    public TimeSpan ClientTimeoutInterval { get; init; } = TimeSpan.FromSeconds(10);

    public static SignalRTimingOptions Read(IConfiguration configuration)
    {
        var options = configuration.GetSection(Section).Get<SignalRTimingOptions>() ?? new SignalRTimingOptions();
        if (options.KeepAliveInterval <= TimeSpan.Zero)
            throw new InvalidOperationException("SignalR__KeepAliveInterval must be positive");
        if (options.ClientTimeoutInterval < 2 * options.KeepAliveInterval)
            throw new InvalidOperationException("SignalR__ClientTimeoutInterval must be at least twice SignalR__KeepAliveInterval");

        return options;
    }
}
