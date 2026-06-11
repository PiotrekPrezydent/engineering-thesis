using Dara.BuildingBlocks.Domain;

namespace Dara.Server.Modules.Clients.Domain.Clients.Rules;

public record ClientSessionMustBeStartedToPreformActionsRule : IBuisnessRule
{
    private readonly bool _isActive;
    private readonly string _action;

    internal ClientSessionMustBeStartedToPreformActionsRule(bool isActive, string action)
    {
        _isActive = isActive;
        _action = action;
    }

    public string Message => $"{nameof(ClientSessionMustBeStartedToPreformActionsRule).TrimEnd("Rule")}";

    public bool IsBroken()
    {
        return _isActive;
    }

}