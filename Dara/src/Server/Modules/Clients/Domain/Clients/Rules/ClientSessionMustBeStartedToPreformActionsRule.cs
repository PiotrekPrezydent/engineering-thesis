using Dara.BuildingBlocks.Domain.Rules;

namespace Dara.Server.Modules.Clients.Domain.Clients.Rules;

public class ClientSessionMustBeStartedToPreformActionsRule : IBuisnessRule
{
    public Client Client;
    public string Action;
    
    internal ClientSessionMustBeStartedToPreformActionsRule(Client client, string action)
    {
        Client = client;
        Action = action;
    }

    public string Message => $"{nameof(ClientSessionMustBeStartedToPreformActionsRule).TrimEnd("Rule")}";

    public bool IsBroken()
    {
        return Client.IsActive;
    }

}