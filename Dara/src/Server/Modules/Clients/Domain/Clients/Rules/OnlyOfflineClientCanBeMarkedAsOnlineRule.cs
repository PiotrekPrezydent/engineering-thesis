using Dara.Server.BuildingBlocks.Domain;

namespace Dara.Server.Modules.Clients.Domain.Clients.Rules;

public record OnlyOfflineClientCanBeMarkedAsOnlineRule : IBuisnessRule
{
    bool _isOnline;
    internal OnlyOfflineClientCanBeMarkedAsOnlineRule(bool isOnline)
    {
        _isOnline = isOnline;
    }

    public string Message => $"{nameof(OnlyOfflineClientCanBeMarkedAsOnlineRule).TrimEnd("Rule")}";

    public bool IsBroken()
    {
        return _isOnline;
    }

}