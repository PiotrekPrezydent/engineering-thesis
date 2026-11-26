namespace Dara.Shared.Contracts;

public static class Connections
{
    // the hub takes a device token only: in the Authorization header or, over WebSocket from a browser,
    // in the access_token query parameter
    public const string HubName = "app";
}
