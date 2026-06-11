using Dara.Shared.Common.Results;
using Dara.Shared.Contracts.Requests;

namespace Dara.Shared.Contracts.Hubs;

public partial interface IAppHub
{
    Task<Result> StartSession(StartSessionRequest request);
    
    Task<Result> EndSession();

    Task<Result> ChangeName();

    Task<Result> AddPlugin();
    
    Task<Result> RemovePlugin();

    Task<Result> AddSupervisor();
    
    Task<Result> RemoveSupervisor();
}