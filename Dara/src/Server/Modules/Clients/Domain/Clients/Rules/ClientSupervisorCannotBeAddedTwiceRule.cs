using Dara.Server.BuildingBlocks.Domain;

namespace Dara.Server.Modules.Clients.Domain.Clients.Rules;

public record ClientSupervisorCannotBeAddedTwiceRule : IBuisnessRule
{
    IReadOnlyList<ClientId> _clientSupervisors;
    ClientId _clientSupervisor;
    
    public ClientSupervisorCannotBeAddedTwiceRule(IReadOnlyList<ClientId> clientSupervisors, ClientId clientSupervisor)
    {
        _clientSupervisors = clientSupervisors;
        _clientSupervisor = clientSupervisor;
    }

    public string Message => nameof(ClientSupervisorCannotBeAddedTwiceRule);
    public bool IsBroken()
    {
        return _clientSupervisors.Contains(_clientSupervisor);
    }
}