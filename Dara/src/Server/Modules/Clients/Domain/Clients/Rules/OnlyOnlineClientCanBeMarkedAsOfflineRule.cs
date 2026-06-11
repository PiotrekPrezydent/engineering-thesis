using Dara.Server.BuildingBlocks.Domain;

namespace Dara.Server.Modules.Clients.Domain.Clients.Rules;

public record OnlyOnlineClientCanBeMarkedAsOfflineRule : IBuisnessRule
{
    bool _isOnline;
    internal OnlyOnlineClientCanBeMarkedAsOfflineRule(bool isOnline)
    {
        _isOnline = isOnline;
    }

    public string Message => $"{nameof(OnlyOnlineClientCanBeMarkedAsOfflineRule).TrimEnd("Rule")}";

    public bool IsBroken()
    {
        return !_isOnline;
    }

}