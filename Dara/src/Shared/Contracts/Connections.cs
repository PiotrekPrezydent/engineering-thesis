namespace Dara.Shared.Contracts;

public static class Connections
{
    // the hub takes a client token only: in the Authorization header or, over WebSocket from a browser,
    // in the access_token query parameter
    public const string HubName = "app";

    // a client pings the server this often and gives it up after this long without a message; the server waits
    // twice its own keep-alive for a client (TASK-ENG-032-D05: 5 s and 10 s on both sides)
    public static readonly TimeSpan ClientKeepAliveInterval = TimeSpan.FromSeconds(5);
    public static readonly TimeSpan ClientServerTimeout = TimeSpan.FromSeconds(10);
}
