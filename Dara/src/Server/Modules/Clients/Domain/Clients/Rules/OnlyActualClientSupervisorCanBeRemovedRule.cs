using Dara.Server.BuildingBlocks.Domain;

namespace Dara.Server.Modules.Clients.Domain.Clients.Rules;

public record OnlyActualClientSupervisorCanBeRemovedRule : IBuisnessRule
{
    IReadOnlyList<ClientId> _clientSupervisors;
    ClientId _clientSupervisor;


    public OnlyActualClientSupervisorCanBeRemovedRule(IReadOnlyList<ClientId> clientSupervisors, ClientId clientSupervisor)
    {
        _clientSupervisors = clientSupervisors;
        _clientSupervisor = clientSupervisor;
    }

    public string Message => nameof(OnlyActualClientSupervisorCanBeRemovedRule);
    public bool IsBroken()
    {
        return !_clientSupervisors.Contains(_clientSupervisor);
    }
}