using Dara.BuildingBlocks.Domain.Rules;

namespace Dara.Server.Modules.Identity.Domain.Clients.Rules;

public class OnlyClientWithSessionCanStopClientSessionRule : IDomainRule
{
    private bool _hasId;
    internal OnlyClientWithSessionCanStopClientSessionRule(bool hasId)
    {
        _hasId = hasId;
    }

    public string Message => $"{nameof(OnlyClientWithSessionCanStopClientSessionRule).TrimEnd("Rule")}";

    public bool IsBroken()
    {
        return !_hasId;
    }

}