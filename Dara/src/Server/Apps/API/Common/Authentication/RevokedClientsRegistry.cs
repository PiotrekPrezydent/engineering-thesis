using System.Collections.Concurrent;

namespace Dara.Server.Apps.API.Common.Authentication;

// clients revoked while their access token may still be valid (up to 15 minutes): the jwt check refuses them at once,
// over http and for a new hub connection; memory is enough, the in-memory databases do not outlive a restart either
public class RevokedClientsRegistry
{
    private readonly ConcurrentDictionary<Guid, byte> _revoked = new();

    public void Add(Guid clientId)
    {
        _revoked.TryAdd(clientId, 0);
    }

    public bool IsRevoked(Guid clientId)
    {
        return _revoked.ContainsKey(clientId);
    }
}
