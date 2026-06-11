using Dara.Shared.Common.Results;

namespace Dara.Shared.Contracts.Hubs;

public partial interface IAppHub
{
    Task GoOnline();
    
    Task GoOffline();

    Task ChangeName(string newName);

    Task AddSupervisor(Guid supervisorId);
    
    Task RemoveSupervisor(Guid supervisorId);
}